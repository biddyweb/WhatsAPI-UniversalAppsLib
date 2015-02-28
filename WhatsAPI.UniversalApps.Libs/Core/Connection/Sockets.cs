using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using WhatsAPI.UniversalApps.Libs.Core.Exceptions;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace WhatsAPI.UniversalApps.Libs.Core.Connection
{
    public class Sockets
    {
        /// <summary>
        /// The time between sending and recieving
        /// </summary>
        private readonly int recvTimeout;

        /// <summary>
        /// The hostname of the whatsapp server
        /// </summary>
        private readonly string whatsHost;

        /// <summary>
        /// The port of the whatsapp server
        /// </summary>
        private readonly int whatsPort;

        /// <summary>
        /// A list of bytes for incomplete messages
        /// </summary>
        private List<byte> incomplete_message = new List<byte>();

        /// <summary>
        /// A socket to connect to the whatsapp network
        /// </summary>
        private StreamSocket socket;

        private uint _transferredSize;
        private DataReader _readerPacket;
        private DataWriter _writePacket;
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }
        /// <summary>
        /// Default class constructor
        /// </summary>
        /// <param name="whatsHost">The hostname of the whatsapp server</param>
        /// <param name="port">The port of the whatsapp server</param>
        /// <param name="timeout">Timeout for the connection</param>
        public Sockets(string whatsHost, int port, int timeout = 2000)
        {
            this.recvTimeout = timeout;
            this.whatsHost = whatsHost;
            this.whatsPort = port;
            this.incomplete_message = new List<byte>();
        }

        /// <summary>
        /// Default class constructor
        /// </summary>
        /// <param name="timeout">Timeout for the connection</param>
        public Sockets(int timeout = 2000)
        {
            this.recvTimeout = timeout;
            this.whatsHost = Constants.Information.WhatsAppHost;
            this.whatsPort = Constants.Information.WhatsPort;
            this.incomplete_message = new List<byte>();
        }

        /// <summary>
        /// Connect to the whatsapp server
        /// </summary>
        public async Task Connect()
        {
            await Task.Run(async () =>
            {
                try
                {


                    this.socket = new StreamSocket();
                    this.socket.Control.KeepAlive = true;
                    _writePacket = new DataWriter(this.socket.OutputStream);
                    _readerPacket = new DataReader(this.socket.InputStream);
                    _readerPacket.InputStreamOptions = InputStreamOptions.Partial;
                    var endPoint = new HostName(this.whatsHost);
                    await this.socket.ConnectAsync(endPoint, this.whatsPort.ToString());

                    //socket.UpgradeToSslAsync(SocketProtectionLevel.SslAllowNullEncryption, new HostName(this.whatsHost));
                    //HandleConnect();
                    _transferredSize = 0;
                    IsConnected = true;

                }
                catch (Exception ex)
                {
                  
                    throw new ConnectionException("Cannot connect");
                }
            });
        }

        /// <summary>
        /// Disconnect from the whatsapp server
        /// </summary>
        public void Disconnect()
        {
            if (this.socket != null)
            {
                this.socket.Dispose();
                IsConnected = false;
            }
        }

        private byte[] _readData;
        /// <summary>
        /// Read 1024 bytes 
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> ReadData()
        {
            List<byte> buff = new List<byte>();
            byte[] ret = await StartReceiving(1024);
            _transferredSize = 0;
            return ret;
        }

        /// <summary>
        /// Send data to the whatsapp server
        /// </summary>
        /// <param name="data">Data to be send as a string</param>
        public async Task SendData(string data)
        {
            var tmpBytes = System.Text.Encoding.GetEncoding(Constants.Information.ASCIIEncoding).GetBytes(data);
            await StartSending(tmpBytes);
        }

        /// <summary>
        /// Send data to the whatsapp server
        /// </summary>
        /// <param name="data">Data to be send as a byte array</param>
        public async Task SendData(byte[] data)
        {
            await StartSending(data);
        }

        /// <summary>
        /// Read in a message with a specific length
        /// </summary>
        /// <param name="length">The lengh of the message</param>
        /// <returns>The recieved data as a byte array</returns>
        private async Task<byte[]> Socket_read(int length)
        {
            if (!IsConnected)
            {
                throw new ConnectionException();
            }

            var buff = new byte[length];
            int receiveLength = 0;

            //try
            //{
            var byteReceived = await StartReceiving((uint)length);
            receiveLength = byteReceived.Length;
            //receiveLength = socket.Receive(buff, 0, length, 0);
            //}
            //catch (Exception excpt)
            //{
            //    if (SocketError.GetStatus(excpt.HResult) == SocketErrorStatus.ConnectionTimedOut)
            //    {
            //        System.Diagnostics.Debug.WriteLine("Connect-TimeOut");
            //        return null;
            //    }
            //    else
            //    {
            //        throw new ConnectionException("Unknown error occured", excpt);
            //    }
            //}

            while (receiveLength <= 0) ;

            byte[] tmpRet = new byte[receiveLength];
            if (receiveLength > 0)
                System.Buffer.BlockCopy(buff, 0, tmpRet, 0, receiveLength);
            return tmpRet;
        }

        public async Task<byte[]> ReadData(int length = 1024)
        {
            return await StartReceiving((uint)length);
        }
        /// <summary>
        /// Sends data of a specific length to the server
        /// </summary>
        /// <param name="data">The data that needs to be send</param>
        /// <param name="length">The lenght of the data</param>
        /// <param name="flags">Optional flags</param>
        private void Socket_send(string data, int length, int flags)
        {
            var tmpBytes = System.Text.Encoding.GetEncoding(Constants.Information.ASCIIEncoding).GetBytes(data);
            SendMsg(tmpBytes);
        }

        /// <summary>
        /// Send data to the server
        /// </summary>
        /// <param name="data">The data that needs to be send as a byte array</param>
        private void Socket_send(byte[] data)
        {
            StartSending(data);
        }

        /// <summary>
        /// Returns the socket status.
        /// </summary>
        public bool SocketStatus
        {
            get { return IsConnected; }
        }

        private void HandleConnect()
        {
            try
            {
                if (!_isConnected)
                {

                }
                else
                {


                    Task readTask = Task.Factory.StartNew(() =>
                    {
                        StartReceiving();
                    });
                }
            }
            catch (Exception ex)
            {


            }
        }

        private async Task<byte[]> StartReceiving(uint length = 1024)
        {
            return await Task.Run(async () =>
                               {
                                   try
                                   {
                                       if (!IsConnected)
                                       {
                                           await Connect();
                                       }

                                       try
                                       {
                                           IAsyncOperation<uint> taskLoad = _readerPacket.LoadAsync(length);
                                           taskLoad.AsTask().Wait();
                                           uint bytesRead = taskLoad.GetResults();
                                           if (bytesRead == 0)
                                               return null;
                                           if (_readerPacket.UnconsumedBufferLength < length)
                                           {
                                                   return await StartReceiving(length);
                                           }
                                           return HandleReceive(bytesRead, _readerPacket, (int)length);
                                       }
                                       catch (Exception ex)
                                       {
                                           WhatsAPI.UniversalApps.Libs.Utils.Logger.Log.WriteLog(ex.Message);
                                           throw ex;
                                       }
                                   }
                                   catch (Exception ex)
                                   {
                                       if (ex.Message.Contains("The operation identifier is not valid."))
                                       {
                                           throw ex;
                                       }
                                       else
                                       {
                                           throw new ConnectionException("Connection Lost");
                                       }
                                   }
                               });

        }

        private byte[] HandleReceive(uint bytesRead, DataReader readPacket, int length = 1024*1024*5)
        {
            var bufferSpace = length * 2;
            byte[] tmpRet = new byte[length];
            Windows.Storage.Streams.IBuffer ibuffer = readPacket.ReadBuffer(readPacket.UnconsumedBufferLength);
            Byte[] convBuffer = WindowsRuntimeBufferExtensions.ToArray(ibuffer);
            
            if (bytesRead > 0)
                System.Buffer.BlockCopy(convBuffer, 0, tmpRet, (int)0, (int)length);
            _transferredSize += bytesRead;
           
            var kucing = System.Text.Encoding.UTF8.GetString(tmpRet, 0, tmpRet.Length);
            WhatsAPI.UniversalApps.Libs.Utils.Logger.Log.WriteLog("Receive Message => " + System.Text.Encoding.UTF8.GetString(tmpRet, 0, tmpRet.Length));
            return tmpRet;
        }

        public void SendMsg(byte[] sendBytes)
        {
            if (!_isConnected)
                Connect();

            Task.Run(async () =>
            {
                await StartSending(sendBytes);
            });
        }
        public async Task<byte[]> ReadNextNode()
        {
            byte[] nodeHeader = await this.ReadData(3);

            if (nodeHeader == null || nodeHeader.Length == 0)
            {
                //empty response
                return null;
            }

            if (nodeHeader.Length != 3)
            {
                throw new Exception("Failed to read node header");
            }
            int nodeLength = 0;
            nodeLength = (int)((nodeHeader[0] & 0x0F) << 16);
            nodeLength |= (int)nodeHeader[1] << 8;
            nodeLength |= (int)nodeHeader[2] << 0;

            //buffered read
            int toRead = nodeLength;
            List<byte> nodeData = new List<byte>();
            do
            {
                byte[] nodeBuff = await this.ReadData(toRead);
                nodeData.AddRange(nodeBuff);
                toRead -= nodeBuff.Length;
            } while (toRead > 0);

            if (nodeData.Count != nodeLength)
            {
                throw new Exception("Read Next Tree error");
            }

            List<byte> buff = new List<byte>();
            buff.AddRange(nodeHeader);
            buff.AddRange(nodeData.ToArray());
            return buff.ToArray();
        }
        //sending message
        private async Task StartSending(byte[] sendBytes)
        {
            await Task.Run(async () =>
            {
                try
                {

                    if (!IsConnected)
                    {
                        await Connect();
                    }
                    WhatsAPI.UniversalApps.Libs.Utils.Logger.Log.WriteLog("Sending Data =>" + System.Text.Encoding.UTF8.GetString(sendBytes, 0, sendBytes.Length));
                    _writePacket.WriteBytes(sendBytes);
                    await _writePacket.StoreAsync();
                }
                catch (Exception ex)
                {


                }
            });
        }
    }
}

