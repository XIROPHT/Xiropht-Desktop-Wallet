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
    }

    public class ClassPeerList
    {
        private const string PeerFileName = "peer-list.json";
        public static int PeerMaxBanTime = 300;
        public static int PeerMaxDisconnect = 50;
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
                            peer_total_disconnect = PeerMaxDisconnect
                        });
                }
                else
                {
                    PeerList[peerHost].peer_status = false;
                    PeerList[peerHost].peer_last_ban = DateTimeOffset.Now.ToUnixTimeSeconds();
                    PeerList[peerHost].peer_total_disconnect = PeerMaxDisconnect;
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
                }
            }


            return PeerList[peerHost].peer_status;
        }

        /// <summary>
        ///     Increment total disconnect of peer host.
        /// </summary>
        /// <param name="peerHost"></param>
        public static void IncrementPeerDisconnect(string peerHost)
        {
            if (!PeerList.ContainsKey(peerHost)) IncludeNewPeer(peerHost);
            PeerList[peerHost].peer_total_disconnect++;
            if (PeerList[peerHost].peer_total_disconnect >= PeerMaxDisconnect) BanPeer(peerHost);
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