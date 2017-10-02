using Neo.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmtool
{

    public class FuncExport
    {
        static string ConvType(string _type)
        {
            switch (_type)
            {
                case "__Signature":
                    return "Signature";

                case "System.Boolean":
                    return "Boolean";

                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                case "System.Numerics.BigInteger":
                    return "Integer";

                case "__Hash160":
                    return "Hash160";

                case "__Hash256":
                    return "Hash256";

                case "System.Byte[]":
                    return "ByteArray";

                case "__PublicKey":
                    return "PublicKey";

                case "System.String":
                    return "String";

                case "System.Object[]":
                    return "Array";

                case "__InteropInterface":
                    return "InteropInterface";

                case "System.Void":
                    return "Void";

                case "System.Object":
                    return "Any";
            }
            if (_type.Contains("[]"))
                return "Array";

            return "Unknown:" + _type;
        }
        public static MyJson.JsonNode_Object Export(AntsModule module)
        {
            var outjson = new MyJson.JsonNode_Object();
            outjson.SetDictValue("hash", "hash");
            outjson.SetDictValue("entrypoint", "Main");

            var funcsigns = new MyJson.JsonNode_Array();
            outjson["functions"] = funcsigns;
            var eventsigns = new MyJson.JsonNode_Array();
            outjson["events"] = eventsigns;

            foreach (var function in module.mapMethods)
            {
                var mm = function.Value;
                if (mm.isPublic == false)
                    continue;
                var ps = mm.name.Split(new char[] { ' ', '(' }, StringSplitOptions.RemoveEmptyEntries);
                var funcsign = new MyJson.JsonNode_Object();

                funcsigns.Add(funcsign);
                var funcname = ps[1];
                if (funcname.IndexOf("::") > 0)
                {
                    var sps = funcname.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                    funcname = sps.Last();
                }
                funcsign.SetDictValue("name", funcname);
                var rtype = ConvType(mm.returntype);
                funcsign.SetDictValue("returntype", rtype);
                MyJson.JsonNode_Array funcparams = new MyJson.JsonNode_Array();
                funcsign["paramaters"] = funcparams;
                if (mm.paramtypes != null)
                {
                    foreach (var v in mm.paramtypes)
                    {
                        var ptype = ConvType(v.type);
                        var item = new MyJson.JsonNode_Object();
                        funcparams.Add(item);

                        item.SetDictValue("name", v.name);
                        item.SetDictValue("type", ptype);
                    }
                }
            }
            return outjson;
        }
    }
}
