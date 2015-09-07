using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Win32;

namespace GISA.Model
{
	public class RegistryHelper
	{

		private static string ProductKeyString
		{
			get
			{
				return string.Format("Software\\{0}\\{1}", ReflectionHelper.CompanyName, ReflectionHelper.ProductName);
			}
		}

		public static RegistryKey getGisaRegistryKey()
		{
			RegistryKey key = null;
			try
			{
				string keyStringPath = ProductKeyString;
                //Console.WriteLine("<<<<<<<P PPP: " + keyStringPath);
				key = Registry.LocalMachine.OpenSubKey(keyStringPath, true);
                //Console.WriteLine("<<<<<<<P PPP: " + "�ss�");
				if (key == null)
				{
					key = Registry.LocalMachine.CreateSubKey(keyStringPath);
				}
			}
			catch (System.Security.SecurityException ex)
			{
				MessageBox.Show("O utilizador n�o possui as permiss�es de leitura do registry" + Environment.NewLine + "necess�rias ao correcto funcionamento da aplica��o. " + Environment.NewLine + "Por favor contacte o administrador de sistema.", "Permiss�es insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Trace.WriteLine("getGisaRegistryKey: " + ex.Message);
                Trace.WriteLine("getGisaRegistryKey: " + ex.StackTrace);
                Trace.WriteLine(ex);
				Environment.Exit(0);
			}
			return key;
		}

		public static string getClientGUID()
		{
			Microsoft.Win32.RegistryKey key = null;
			key = getGisaRegistryKey();
			string clientGUID = (string)(key.GetValue("Identifier"));
			if (clientGUID == null)
			{
				clientGUID = Guid.NewGuid().ToString();
				key.SetValue("Identifier", clientGUID);
			}
			return clientGUID;
		}
	}
}