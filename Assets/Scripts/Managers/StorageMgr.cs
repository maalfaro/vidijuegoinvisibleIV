using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;
using System;

public interface IDataStorage<T> { string SAVE_FILE { get; } void SaveData(); T LoadData(); }

public class StorageMgr {

	/// <summary>
	/// Metodo que persiste un archivo binario del tiempo que se le pasa como objeto T y con el nombre que le pasamos por parametro
	/// Ej: SaveData(GameData, "UG_data");
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="data"></param>
	/// <param name="dataName"></param>
	public static void SaveData<T>(T data, string dataName)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream stream = new FileStream(Application.persistentDataPath +"/"+ dataName + ".dat", FileMode.Create);

		bf.Serialize(stream, Encrypt(JsonUtility.ToJson(data)));
		stream.Close();

	}

	/// <summary>
	/// Metodo que carga un archivo de la ruta de persistencia.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="dataName"></param>
	/// <returns></returns>
	public static T LoadData<T>(string dataName)
	{
		string datapath = Application.persistentDataPath + "/" + dataName + ".dat";
		if (File.Exists(datapath))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream stream = new FileStream(datapath, FileMode.Open);

			T data = JsonUtility.FromJson<T>(Decrypt(bf.Deserialize(stream).ToString()));
			stream.Close();

			return data;
		}

		throw new System.Exception("El archivo no existe");
	}

	public static string Encrypt(string toEncrypt)
	{
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");
		// 256-AES key
		byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
		RijndaelManaged rDel = new RijndaelManaged();
		rDel.Key = keyArray;
		rDel.Mode = CipherMode.ECB;
		rDel.Padding = PaddingMode.PKCS7;
		// better lang support
		ICryptoTransform cTransform = rDel.CreateEncryptor();
		byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		return Convert.ToBase64String(resultArray, 0, resultArray.Length);
	}

	public static string Decrypt(string toDecrypt)
	{
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");
		// AES-256 key
		byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
		RijndaelManaged rDel = new RijndaelManaged();
		rDel.Key = keyArray;
		rDel.Mode = CipherMode.ECB;
		rDel.Padding = PaddingMode.PKCS7;
		// better lang support
		ICryptoTransform cTransform = rDel.CreateDecryptor();
		byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		return UTF8Encoding.UTF8.GetString(resultArray);
	}
}
