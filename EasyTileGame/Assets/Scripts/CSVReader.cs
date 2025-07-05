using System.Collections.Generic;
using System.IO;
using UnityEngine;

// CSV 파일을 읽어들이는 스크립트
public class CSVReader : MonoBehaviour
{
	public static List<Dictionary<string, string>> ReadCSV(string CSVFileName)
	{
		StreamReader streamReader = new StreamReader(Application.dataPath + "/CSVFile/" + CSVFileName);
		List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
		List<string> listName = new List<string>();

		bool endLine = false;

		string dataFirst = streamReader.ReadLine();
		var value = dataFirst.Split('@');

		for (int i = 0; i < value.Length; i++)
		{
			listName.Add(value[i].ToString());
		}

		while (!endLine)
		{
			Dictionary<string, string> itemDataDic = new Dictionary<string, string>();

			string dataStr = streamReader.ReadLine();
			if (dataStr == null)
			{
				endLine = true;
				break;
			}

			value = dataStr.Split('@');
			for (int i = 0; i < value.Length; i++)
			{
				itemDataDic.Add(listName[i], value[i]);
			}

			list.Add(itemDataDic);
		}

		return list;
	}
}
