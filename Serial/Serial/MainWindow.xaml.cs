using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.IO.Ports;
using System.Collections.Concurrent;
using System.Windows.Threading;
using System.Threading;

namespace Serial
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort _serialport;
        private DateTime _datetime;
        private int[] frecuencias;
        private String comparacion;
        public MainWindow()
        {
            InitializeComponent();
            frecuencias = new int[]{4800,9600,19200,38400,57600,115200,230400,250000};
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames())
            {
                comboPuertos.Items.Add(s);
            }

            comboFrecuencia.ItemsSource = frecuencias;
            
        }


        private void conectar_puertoserial(String puerto, int frecuencia) 
        {
            _serialport = new SerialPort(puerto, frecuencia, Parity.None, 8, StopBits.One);

            String formato = string.Format("Conectado");
            try
            {
                _serialport.Open();
                actualiza_consolaUI(formato);
                _serialport.DataReceived += new SerialDataReceivedEventHandler(SerialPortDataReceived);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Error"); }
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
             String reciberx = _serialport.ReadExisting();
             

             if (reciberx.Length > 2 && reciberx != comparacion)
             {
                 String formato = string.Format("Recibido:{0}", reciberx.TrimEnd());
                actualiza_consolaUI(formato);
                comparacion = reciberx;
             }
        
        }

        private void btn_conectar_Click(object sender, RoutedEventArgs e)
        {
            String puerto = comboPuertos.Text;
            int frecuencia = Convert.ToInt32(comboFrecuencia.Text);
           
            conectar_puertoserial(puerto, frecuencia);
        }

        private void actualiza_consolaUI(String s) {
            Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new ThreadStart(delegate
                    {
                        _datetime = DateTime.Now;
                        String formato = String.Format("[{0}]{1}\n", _datetime, s);
                        txtrecibe.AppendText(formato);
                        txtrecibe.Focus();                             //Obtiene el focus
                        txtrecibe.CaretIndex = txtrecibe.Text.Length;  //Obtiene el tamaño de la caja
                        txtrecibe.ScrollToEnd();                       //setea scroll al final
                    }));
        }

        private void btn_enviar_Click(object sender, RoutedEventArgs e)
        {
            String dato_a_enviar = txt_enviar.Text;
            _serialport.Write(dato_a_enviar);          
            String formato = string.Format("Enviado:{0}", dato_a_enviar);    

            actualiza_consolaUI(formato);
        }

        private void btn_desconectar_Click(object sender, RoutedEventArgs e)
        {
            if (_serialport.IsOpen)
            {
                _serialport.Close();               
                actualiza_consolaUI("Desconectado");
            }
        }



    }


}
