
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace Xiropht_Wallet
{
    public class ClassWalletRestoreFunctions : IDisposable
    {
        /// <summary>
        /// Dispose information.
        /// </summary>
        private bool IsDisposed;

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
            if (IsDisposed)
                return;

            IsDisposed = true;
        }

        #endregion


        /// <summary>
        /// Generate QR Code from private key + password, encrypt the QR Code bitmap with the private key, build the request to be send on the blockchain.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GenerateQRCodeKeyEncryptedRepresentation(string privateKey, string password)
        {
            try
            {
                QrCodeEncodingOptions options = new QrCodeEncodingOptions
                {
                    DisableECI = true,
                    CharacterSet = "UTF-8",
                    Width = 2,
                    Height = 2,
                };

                BarcodeWriter qr = new BarcodeWriter
                {
                    Options = options,
                    Format = BarcodeFormat.QR_CODE
                };
                string sourceKey = privateKey.Trim() + "|" + password.Trim();
                using (var representationQRCode = new Bitmap(qr.Write(sourceKey)))
                {

                    LuminanceSource source = new BitmapLuminanceSource(representationQRCode);

                    BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                    Result result = new MultiFormatReader().decode(bitmap);

                    if (result != null)
                    {
                        if (result.Text == sourceKey)
                        {

                            string qrCodeString = BitmapToBase64String(representationQRCode);
                            string QrCodeStringEncrypted = ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael, qrCodeString, privateKey, ClassWalletNetworkSetting.KeySize);
                            string qrCodeEncryptedRequest = string.Empty;

                            if (privateKey.Contains("$"))
                            {
                                long walletUniqueIdInstance = long.Parse(privateKey.Split(new[] { "$" }, StringSplitOptions.None)[1]);
                                qrCodeEncryptedRequest = walletUniqueIdInstance + "|" + QrCodeStringEncrypted;
                            }
                            else
                            {

                                string randomEndPrivateKey = privateKey.Remove(0, (privateKey.Length - ClassUtils.GetRandomBetween(privateKey.Length / 4, privateKey.Length / 8))); // Indicate only a small part of the end of the private key (For old private key users).
                                qrCodeEncryptedRequest = randomEndPrivateKey + "|" + QrCodeStringEncrypted;
                            }
                            string decryptQrCode = ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael, QrCodeStringEncrypted, privateKey, ClassWalletNetworkSetting.KeySize);

                            using (Bitmap qrCode = Base64StringToBitmap(decryptQrCode))
                            {

                                source = new BitmapLuminanceSource(qrCode);

                                bitmap = new BinaryBitmap(new HybridBinarizer(source));
                                result = new MultiFormatReader().decode(bitmap);

                                if (result != null)
                                {
                                    if (result.Text == sourceKey)
                                    {
                                        return qrCodeEncryptedRequest;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception error)
            {
#if DEBUG
                Console.WriteLine("error: " + error.Message);
#endif
            }
            return null;
        }

        /// <summary>
        /// Convert a bitmap into byte array then in base64 string.
        /// </summary>
        /// <param name="newImage"></param>
        /// <returns></returns>
        public string BitmapToBase64String(Bitmap newImage)
        {

            Bitmap bImage = newImage;  
            using (MemoryStream ms = new MemoryStream())
            {
                bImage.Save(ms, ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();
                return Convert.ToBase64String(byteImage);
            }
        }

        /// <summary>
        /// Convert a base64 string into byte array, then into bitmap.
        /// </summary>
        /// <param name="stringImage"></param>
        /// <returns></returns>
        public Bitmap Base64StringToBitmap(string stringImage)
        {
            byte[] byteStringImage = Convert.FromBase64String(stringImage);

            using (var ms = new MemoryStream(byteStringImage))
            {
                return new Bitmap(ms);
            }
        }

    }
}
