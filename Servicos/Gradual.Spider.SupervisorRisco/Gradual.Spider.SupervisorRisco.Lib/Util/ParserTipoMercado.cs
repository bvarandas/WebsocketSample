using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Util
{
    public class ParserTipoMercado
    {

        public static string Segmento2TipoMercado(string segmento, string indicadoropcao)
        {
            string ret = string.Empty;

            switch (segmento)
            {
                case "01":
                case "91":
                    ret = "VIS";
                    break;
                case "09":
                    if (indicadoropcao.Equals("C"))
                        ret = "EOC";
                    if (indicadoropcao.Equals("P"))
                        ret = "EOV";
                    break;
                case "05":
                    ret = "LEI";
                    break;
                case "06":
                    ret = "LNC";
                    break;
                case "03":
                    ret = "FRA";
                    break;
                case "02":
                    ret = "TER";
                    break;
                case "FUT":
                    ret = "FUT";
                    break;
                case "04":
                    if (indicadoropcao.Equals("C"))
                        ret = "OPC";
                    if (indicadoropcao.Equals("P"))
                        ret = "OPV";
                    break;
                case "SPOT":
                    ret = "DIS";
                    break;
                case "SOPT":
                    ret = "OPD";
                    break;
                case "FOPT":
                    ret = "OPF";
                    break;
            }


            return ret;
        }


    }
}
