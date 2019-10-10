using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CombineDCS
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] inPaths = args;
            WriteData(inPaths,"out.csv");         
        }

        /// <summary>
        /// 合并写入文件
        /// </summary>
        /// <param name="inPaths"></param>
        /// <param name="outPath"></param>
        static void WriteData(string[] inPaths,string outPath)
        {
            var listTempH = new List<List<string>>();
            var listTempI = new List<List<Dictionary<string, string>>>();
            for (int i = 0; i < inPaths.Length; i++)
            {
                inPaths[i]= Path.Combine(Environment.CurrentDirectory, inPaths[i]);
                var listh = ReadDataHeader(inPaths[i]);
                var listitem = ReadDataItem(inPaths[i]);
                listTempH.Add(listh);
                listTempI.Add(listitem);
            }
            var listH = listTempH[0];
            for (int i = 0; i < listTempH.Count-1; i++)
            {
                listH = listH.Union(listTempH[i+1]).ToList<string>();
            }
            var listI = listTempI[0];
            for (int i = 0; i < listTempI.Count - 1; i++)
            {
                listI = listI.Concat(listTempI[i+1]).ToList<Dictionary<string, string>>();
            }
            var listResult = DataCombine(listH, listI);
            using (var sw = new StreamWriter(outPath))
            {
                foreach (var p in listResult)
                {
                    foreach (var t in p)
                    {
                        sw.Write(t + ",");
                    }
                    sw.WriteLine();
                }
            }
                        
        }

        /// <summary>
        /// 表头和表内容合并
        /// </summary>
        /// <param name="listH"></param>
        /// <param name="listItem"></param>
        /// <returns></returns>
        static List<List<string>> DataCombine(List<string> listH,List<Dictionary<string,string>> listItem)
        {
            var listResult = new List<List<string>>();
            listResult.Add(listH);
            foreach (var item in listItem)
            {
                var listtemp = new List<string>();
                foreach (var k in listH)
                {
                    listtemp.Add(item.ContainsKey(k) ? item[k] : "0");
                }
                listResult.Add(listtemp);
            }
            return listResult;
        }

        /// <summary>
        /// 读表头
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static List<string> ReadDataHeader(string path)
        {
            using (var sr = new StreamReader(path))
            {               
                var line= sr.ReadLine();
                return line.Split(',').ToList();
            }
        }

        /// <summary>
        /// 读数据行
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static List<Dictionary<string,string>> ReadDataItem(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var line = "";
                var listH= sr.ReadLine().Split(',').ToList();
                var llist = new List<Dictionary<string,string>>();
                while ((line = sr.ReadLine()) != null)
                {
                    var listItem=line.Split(',').ToList();
                    var dic = new Dictionary<string, string>();
                    for(int i= 0;i < listH.Count;i++)
                    {
                        dic.Add(listH[i],listItem[i]);
                    }
                    llist.Add(dic);
                }
                return llist;
            }
        }

    }
}
