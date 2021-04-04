﻿using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace ClientInterface
{
    public partial class ClientForm : Form
    {
        TcpClient client = null;
        string userName;
        public ClientForm()
        {
            InitializeComponent();
            AutoCompleteStringCollection source = new AutoCompleteStringCollection(){"8888"};
            textBoxPort.AutoCompleteCustomSource = source;
            textBoxPort.AutoCompleteMode = AutoCompleteMode.Append;
            textBoxPort.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }
        //при клике на кнопку войти происходит попытка TCP-клиента соединиться с сервером
        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            LogIn();
        }
        //попытка TCP-клиента соединиться с сервером
        public void LogIn()
        {
            try
            {
                userName = textBoxName.Text;
                client = new TcpClient(textBoxAddress.Text, int.Parse(textBoxPort.Text));
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.Unicode.GetBytes(String.Format(userName + ": вошёл в чат "));
                stream.Write(data, 0, data.Length);
                Thread ReceThread = new Thread(Receive);
                ReceThread.Start();
                buttonSend.Enabled = true;
            }
            catch
            {
                MessageBox.Show(
                "Мы не можем установить надёжного соединения",
                "Сообщение",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        //поток получения и обработки сообщений от сервера 
        public void Receive()
        {
            NetworkStream stream = client.GetStream();
            while (true)
            {
                byte[] data = new byte[1024];
                int bytes = stream.Read(data, 0, data.Length);
                string message = Encoding.Unicode.GetString(data, 0, bytes);
                if (!OnlineClient(message))
                {
                    textBoxChat.AppendText(message);
                    textBoxChat.AppendText(Environment.NewLine);
                }
            }
        }
        /*Обновляет информацию у TCP-клиента, о ползователях онлайн
         true - если это комнада об обновленни пользователей 
         false - если это не комнада об обновленни пользователей */
        public bool OnlineClient(string user)
        {
            if (user[0] == '/')
            {
                user = user.Substring(5);
                string[] words = user.Split('#');
                textBoxUserList.Clear();
                foreach (var word in words)
                {
                    textBoxUserList.AppendText(word);
                    textBoxUserList.AppendText(Environment.NewLine);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        //Отправляет на сервер массив байт
        public void Send()
        {
            NetworkStream stream = client.GetStream();
            string message = textBoxMsg.Text;
            textBoxChat.AppendText("Вы: " + message);
            textBoxChat.AppendText(Environment.NewLine);
            byte[] data = Encoding.Unicode.GetBytes(String.Format("{0}: {1}", userName, message));
            stream.Write(data, 0, data.Length);
            textBoxMsg.Clear();
        }
        //при клике на кнопку отправить отправляется массив байт на сервер
        private void buttonSend_Click(object sender, EventArgs e)
        {
            Send();
        }
        //Очистка поле ввода при клике на них
        private void textBoxAddress_Click(object sender, EventArgs e)
        {
            textBoxAddress.Clear();
        }
        private void textBoxPort_Click(object sender, EventArgs e)
        {
            textBoxPort.Clear();
        }
        private void textBoxName_Click(object sender, EventArgs e)
        {
            textBoxName.Clear();
        }

        //при нажатии кнопки Enter откправлет массив байт на сервер 
        private void textBoxMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Send();
            }
        }
        //при закрытии окна отправляет серверу команду об отключении клиента
        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                string message = "/Close";
                byte[] data = Encoding.Unicode.GetBytes(String.Format(message));
                stream.Write(data, 0, data.Length);
            }
            catch
            {
                return;
            }
        }
        //при нажатии кнопки Enter переключает фокус на поле с вводом порта
        private void textBoxAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ActiveControl = textBoxPort;
            }
        }
        //при нажатии кнопки Enter переключает фокус на поле с вводом имени
        private void textBoxPort_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ActiveControl = textBoxName;
            }
        }
        //при нажатии кнопки Enter происходит попытка подключения к серверу
        private void textBoxName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                LogIn();
            }
        }
    }
}
