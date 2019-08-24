using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace Xiropht_Wallet.Wallet.Tcp.Option
{
    public class ClassWalletRestoreFunctions : IDisposable
    {
        /// <summary>
        ///     Dispose information.
        /// </summary>
        private bool _isDisposed;


        /// <summary>
        ///     Generate QR Code from private key + password, encrypt the QR Code bitmap with the private key, build the request to
        ///     be send on the blockchain.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GenerateQrCodeKeyEncryptedRepresentation(string privateKey, string password)
        {
            try
            {
                var options = new QrCodeEncodingOptions
                {
                    DisableECI = true,
                    CharacterSet = "UTF-8",
                    Width = 2,
                    Height = 2
                };

                var qr = new BarcodeWriter
                {
                    Options = options,
                    Format = BarcodeFormat.QR_CODE
                };
                var sourceKey = privateKey.Trim() + ClassConnectorSetting.PacketContentSeperator + password.Trim() + ClassConnectorSetting.PacketContentSeperator + ClassUtils.DateUnixTimeNowSecond();
                using (var representationQrCode = new Bitmap(qr.Write(sourceKey)))
                {
                    LuminanceSource source = new BitmapLuminanceSource(representationQrCode);

                    var bitmap = new BinaryBitmap(new HybridBinarizer(source));
                    var result = new MultiFormatReader().decode(bitmap);

                    if (result != null)
                        if (result.Text == sourceKey)
                        {
                            var qrCodeString = BitmapToBase64String(representationQrCode);
                            var QrCodeStringEncrypted = ClassAlgo.GetEncryptedResultManual(
                                ClassAlgoEnumeration.Rijndael, qrCodeString, privateKey,
                                ClassWalletNetworkSetting.KeySize);
                            var qrCodeEncryptedRequest = string.Empty;

                            if (privateKey.Contains("$"))
                            {
                                var walletUniqueIdInstance =
                                    long.Parse(privateKey.Split(new[] {"$"}, StringSplitOptions.None)[1]);
                                qrCodeEncryptedRequest = walletUniqueIdInstance + ClassConnectorSetting.PacketContentSeperator + QrCodeStringEncrypted;
                            }
                            else
                            {
                                var randomEndPrivateKey = privateKey.Remove(0,
                                    privateKey.Length -
                                    ClassUtils.GetRandomBetween(privateKey.Length / 4,
                                        privateKey.Length /
                                        8)); // Indicate only a small part of the end of the private key (For old private key users).
                                qrCodeEncryptedRequest = randomEndPrivateKey + ClassConnectorSetting.PacketContentSeperator + QrCodeStringEncrypted;
                            }

                            // Testing QR Code encryption.
                            var decryptQrCode = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael,
                                QrCodeStringEncrypted, privateKey,
                                ClassWalletNetworkSetting.KeySize); // Decrypt QR Code.

                            using (var qrCode = Base64StringToBitmap(decryptQrCode)) // Retrieve data to bitmap.
                            {
                                source = new BitmapLuminanceSource(qrCode);

                                bitmap = new BinaryBitmap(new HybridBinarizer(source));
                                result = new MultiFormatReader().decode(bitmap);

                                if (result != null)
                                    if (result.Text == sourceKey) // Check representation.
                                        return qrCodeEncryptedRequest; // Return encrypted QR Code.
                            }
                        }
                }
            }
            catch (Exception error)
            {
#if DEBUG
                Console.WriteLine("error: " + error.Message);
#endif
            }

            return null;
        }

        /// <summary>
        ///     Convert a bitmap into byte array then in base64 string.
        /// </summary>
        /// <param name="newImage"></param>
        /// <returns></returns>
        public string BitmapToBase64String(Bitmap newImage)
        {
            var bImage = newImage;
            using (var ms = new MemoryStream())
            {
                bImage.Save(ms, ImageFormat.Jpeg);
                var byteImage = ms.ToArray();
                return Convert.ToBase64String(byteImage);
            }
        }

        /// <summary>
        ///     Convert a base64 string into byte array, then into bitmap.
        /// </summary>
        /// <param name="stringImage"></param>
        /// <returns></returns>
        public Bitmap Base64StringToBitmap(string stringImage)
        {
            var byteStringImage = Convert.FromBase64String(stringImage);

            using (var ms = new MemoryStream(byteStringImage))
            {
                return new Bitmap(ms);
            }
        }

        #region Dispose functions

        ~ClassWalletRestoreFunctions()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
        }

        #endregion
    }
}