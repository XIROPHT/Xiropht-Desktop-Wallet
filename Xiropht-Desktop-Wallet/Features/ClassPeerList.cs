using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Xiropht_Connector_All.Setting;
using Xiropht_Wallet.Utility;

namespace Xiropht_Wallet.Features
{
    public class ClassPeerObject
    {
        public string peer_host = string.Empty;
        public long peer_last_ban;
        public bool peer_status;
        public int peer_total_disconnect;
        public int peer_trust_value;
        public long peer_last_trust;
        public bool peer_proxy_status;
        public long peer_last_proxy_ban;
    }

    public class ClassPeerList
    {
        private const string PeerFileName = "peer-list.json";
        public static int PeerMaxBanTime = 300;
        public static int PeerMaxDisconnect = 50;
        public static int PeerTrustPoint = 1;
        public static int PeerMaxTimeTrust = 30;
        public static int PeerTrustMinimumValue = 50;
        public static bool PeerEnableTrustSystem = false;
        public static Dictionary<string, ClassPeerObject> PeerList = new Dictionary<string, ClassPeerObject>();

        /// <summary>
        ///     Load peer list.
        /// </summary>
        public static void LoadPeerList()
        {
            if (File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + PeerFileName)))
                using (var reader =
                    new StreamReader(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + PeerFileName)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        try
                        {
                            var peerObject = JsonConvert.DeserializeObject<ClassPeerObject>(line);
                            if (IPAddress.TryParse(peerObject.peer_host, out _))
                            {
                                if (!PeerList.ContainsKey(peerObject.peer_host))
                                    PeerList.Add(peerObject.peer_host, peerObject);
                            }
                        }
                        catch
                        {
                        }
                }
            else
                File.Create(AppDomain.CurrentDomain.BaseDirectory + PeerFileName).Close();
        }

        /// <summary>
        ///     Include a new peer.
        /// </summary>
        /// <param name="peerHost"></param>
        public static void IncludeNewPeer(string peerHost)
        {
            if (!PeerList.ContainsKey(peerHost))
                if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(peerHost))
                PeerList.Add(peerHost, new ClassPeerObject {peer_host = peerHost, peer_status = true});
        }

        /// <summary>
        ///     Ban a peer, insert it if he is a new peer.
        /// </summary>
        /// <param name="peerHost"></param>
        public static void BanPeer(string peerHost)
        {
            if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(peerHost))
            {
                if (!PeerList.ContainsKey(peerHost))
                {
                    PeerList.Add(peerHost,
                        new ClassPeerObject
                        {
                            peer_host = peerHost, peer_status = false,
                            peer_last_ban = DateTimeOffset.Now.ToUnixTimeSeconds(),
                            peer_total_disconnect = PeerMaxDisconnect,
                            peer_trust_value = 0,
                            peer_last_trust = 0
                        });
                }
                else
                {
                    PeerList[peerHost].peer_status = false;
                    PeerList[peerHost].peer_last_ban = DateTimeOffset.Now.ToUnixTimeSeconds();
                    PeerList[peerHost].peer_total_disconnect = PeerMaxDisconnect;
                    PeerList[peerHost].peer_trust_value = 0;
                    PeerList[peerHost].peer_last_trust = 0;
                }
            }
        }

        /// <summary>
        ///     Get the peer status.
        /// </summary>
        /// <param name="peerHost"></param>
        /// <returns></returns>
        public static bool GetPeerStatus(string peerHost)
        {
            if (!PeerList.ContainsKey(peerHost))
            {
                
                IncludeNewPeer(peerHost);
                return true;
            }

            if (!PeerList[peerHost].peer_status)
            {
                if (PeerList[peerHost].peer_last_ban + PeerMaxBanTime <= DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    PeerList[peerHost].peer_status = true;
                    PeerList[peerHost].peer_total_disconnect = 0;
                    PeerList[peerHost].peer_trust_value = 0;
                    PeerList[peerHost].peer_last_trust = 0;
                }
            }


            return PeerList[peerHost].peer_status;
        }

        /// <summary>
        /// Get the peer proxy status.
        /// </summary>
        /// <param name="peerHost"></param>
        /// <returns></returns>
        public static bool GetPeerProxyStatus(string peerHost)
        {
            if (!PeerList.ContainsKey(peerHost))
            {

                IncludeNewPeer(peerHost);
                return true;
            }

            if (!PeerList[peerHost].peer_proxy_status)
            {
                if (PeerList[peerHost].peer_last_proxy_ban + PeerMaxBanTime <= DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    PeerList[peerHost].peer_proxy_status = true;
                    PeerList[peerHost].peer_last_proxy_ban = 0;
                }
            }


            return PeerList[peerHost].peer_status;
        }

        /// <summary>
        /// Ban proxy peer.
        /// </summary>
        /// <param name="peerHost"></param>
        public static void BanProxyPeer(string peerHost)
        {
            if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(peerHost))
            {
                if (!PeerList.ContainsKey(peerHost))
                {
                    PeerList.Add(peerHost,
                        new ClassPeerObject
                        {
                            peer_host = peerHost,
                            peer_proxy_status = false,
                            peer_last_proxy_ban = DateTimeOffset.Now.ToUnixTimeSeconds()
                        });
                }
                else
                {
                    PeerList[peerHost].peer_proxy_status = false;
                    PeerList[peerHost].peer_last_proxy_ban = DateTimeOffset.Now.ToUnixTimeSeconds();
                }
            }
        }

        /// <summary>
        ///     Increment total disconnect of peer host.
        /// </summary>
        /// <param name="peerHost"></param>
        public static void IncrementPeerDisconnect(string peerHost)
        {
            if (!PeerList.ContainsKey(peerHost)) IncludeNewPeer(peerHost);
            PeerList[peerHost].peer_total_disconnect++;
            if (PeerList[peerHost].peer_trust_value > 0)
            {
                PeerList[peerHost].peer_trust_value--;
            }
            else
            {
                PeerList[peerHost].peer_trust_value = 0;
            }
            if (PeerList[peerHost].peer_total_disconnect >= PeerMaxDisconnect) BanPeer(peerHost);
        }

        /// <summary>
        /// Increment trust value of the peer.
        /// </summary>
        /// <param name="peerHost"></param>
        public static void IncrementPeerTrustPoint(string peerHost)
        {
            if (!PeerList.ContainsKey(peerHost)) IncludeNewPeer(peerHost);
            if (PeerList[peerHost].peer_trust_value < PeerMaxTimeTrust)
            {
                PeerList[peerHost].peer_trust_value += PeerTrustPoint;
            }
            if (PeerList[peerHost].peer_trust_value >= PeerMaxTimeTrust)
            {
                if (PeerList[peerHost].peer_last_trust < DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    PeerList[peerHost].peer_last_trust = DateTimeOffset.Now.ToUnixTimeSeconds() + PeerMaxTimeTrust;
                }
            }
        }

        /// <summary>
        /// Check if the peer is trusted.
        /// </summary>
        /// <param name="peerHost"></param>
        /// <returns></returns>
        public static bool GetPeerTrustStatus(string peerHost)
        {
            if (ClassConnectorSetting.SeedNodeIp.ContainsKey(peerHost))
            {
                return true;
            }
            if (!PeerList.ContainsKey(peerHost)) IncludeNewPeer(peerHost);

            if (PeerEnableTrustSystem)
            {
                if (PeerList.ContainsKey(peerHost))
                {
                    if (PeerList[peerHost].peer_trust_value >= PeerMaxTimeTrust)
                    {
                        if (PeerList[peerHost].peer_last_trust >= DateTimeOffset.Now.ToUnixTimeSeconds())
                        {
                            if (PeerList[peerHost].peer_last_trust - DateTimeOffset.Now.ToUnixTimeSeconds() <=
                                PeerMaxTimeTrust)
                            {
                                if (PeerList[peerHost].peer_trust_value - PeerList[peerHost].peer_total_disconnect >=
                                    ((PeerTrustMinimumValue * 50) / 100))
                                {
                                    return true;
                                }
                            }
                        }

                        PeerList[peerHost].peer_trust_value = 0;
                        PeerList[peerHost].peer_last_trust = 0;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Save peer list.
        /// </summary>
        public static void SavePeerList()
        {
            try
            {
                File.Create(AppDomain.CurrentDomain.BaseDirectory + PeerFileName).Close();

                using (var writer =
                    new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + PeerFileName)))
                {
                    foreach (var peer in PeerList.ToArray())
                    {
                        var peerData = JsonConvert.SerializeObject(peer.Value);
                        writer.WriteLine(peerData);
                    }
                }
            }
            catch
            {
            }
        }
    }
}