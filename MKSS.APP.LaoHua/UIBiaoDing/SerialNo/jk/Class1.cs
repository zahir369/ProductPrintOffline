using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKSS.APP.LaoHua.UIBiaoDing.SerialNo.jk
{


    public class Vef
    {
        public static int GetGB17710(string str)
        {
            string strTmp;
            int[] aArray, pArray, sArray;
            int iLen, i, j;
            aArray = new int[17];
            pArray = new int[17];
            sArray = new int[17];

            strTmp = str;
            iLen = strTmp.Length;
            j = iLen - 1;
            aArray[0] = 0;
            for (i = 2; i <= iLen; i++)
            {
                string sNum = strTmp[j].ToString();
                aArray[i] = Convert.ToInt32(sNum);
                j--;
            }
            j = 0;
            for (i = 16; i > 1; i--)
            {
                j++;
                if (j == 1)
                {
                    pArray[j] = 10;
                }
                else
                {
                    pArray[j] = (sArray[j - 1] % 10) * 2;
                }
                if (pArray[j] == 0)
                {
                    pArray[j] = 10;
                    pArray[j] = pArray[j] * 2;
                }
                sArray[j] = pArray[j] % 11;
                if (sArray[j] == 0)
                {
                    sArray[j] = 10;
                }
                sArray[j] = sArray[j] + aArray[i];
            }
            iLen++;
            pArray[iLen] = (sArray[j] % 10) * 2;
            aArray[1] = 10 - ((pArray[iLen] - 1) % 10);
            if (aArray[1] >= 10)
            {
                aArray[1] = 0;
            }
            return aArray[1];
        }
    }
}
