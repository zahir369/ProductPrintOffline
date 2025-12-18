using SkiaSharp;
using System;
using System.Windows.Media;

namespace MKSS.APP.ECTester.UserCommon
{


    public class SKColorUtil {
        static SKColorUtil of = new SKColorUtil() {
            
        };
        public static SKColorUtil Of
        {
            get {
                return of;
            }
        }

        public SKColorUtil() {
            ALL = new SKColor[] {
                LightPink,Pink,Crimson,LavenderBlush,PaleVioletRed,HotPink,DeepPink,MediumVioletRed,Orchid,Thistle,Plum,Violet,Magenta,Fuchsia,DarkMagenta,Purple,MediumOrchid,DarkViolet,DarkOrchid,Indigo,BlueViolet,MediumPurple,MediumSlateBlue,SlateBlue,DarkSlateBlue,Lavender,GhostWhite,Blue,MediumBlue,MidnightBlue,DarkBlue,Navy,RoyalBlue,CornflowerBlue,LightSteelBlue,LightSlateGray,SlateGray,DodgerBlue,AliceBlue,SteelBlue,LightSkyBlue,SkyBlue,DeepSkyBlue,LightBlue,PowderBlue,CadetBlue,Azure,LightCyan,PaleTurquoise,Cyan,Aqua,DarkTurquoise,DarkSlateGray,DarkCyan,Teal,MediumTurquoise,LightSeaGreen,Turquoise,Aquamarine,MediumAquamarine,MediumSpringGreen,MintCream,SpringGreen,MediumSeaGreen,SeaGreen,Honeydew,LightGreen,PaleGreen,DarkSeaGreen,LimeGreen,Lime,ForestGreen,Green,DarkGreen,Chartreuse,LawnGreen,GreenYellow,DarkOliveGreen,YellowGreen,OliveDrab,Beige,LightGoldenrodYellow,Ivory,LightYellow,Yellow,Olive,DarkKhaki,LemonChiffon,PaleGoldenrod,Khaki,Gold,Cornsilk,Goldenrod,DarkGoldenrod,FloralWhite,OldLace,Wheat,Moccasin,Orange,PapayaWhip,BlanchedAlmond,NavajoWhite,AntiqueWhite,Tan,BurlyWood,Bisque,DarkOrange,Linen,Peru,PeachPuff,SandyBrown,Chocolate,SaddleBrown,Seashell,Sienna,LightSalmon,Coral,OrangeRed,DarkSalmon,Tomato,MistyRose,Salmon,Snow,LightCoral,RosyBrown,IndianRed,Red,Brown,FireBrick,DarkRed,Maroon,White,WhiteSmoke,Gainsboro,LightGrey,Silver,DarkGray,Gray,DimGray,Black
            };
        } 

        /// <summary>
        /// 随机神色
        /// </summary>
        public SKColor RandomDark {
            get {
                byte[] buffer = Guid.NewGuid().ToByteArray();
                int iSeed = BitConverter.ToInt32(buffer, 0);
                Random random = new Random(iSeed);
                SKColor r = ALL[random.Next(0, ALL.Length)];
                while (r.Red + r.Green + r.Blue > 450) {
                    r = ALL[random.Next(0, ALL.Length)];
                }
                return r;
            }
        }

        /// <summary>
        /// 随机神色
        /// </summary>
        public SKColor RandomNoDark
        {
            get
            {
                byte[] buffer = Guid.NewGuid().ToByteArray();
                int iSeed = BitConverter.ToInt32(buffer, 0);
                Random random = new Random(iSeed);
                SKColor r = ALL[random.Next(0, ALL.Length)];
                while (r.Red + r.Green + r.Blue < 100)
                {
                    r = ALL[random.Next(0, ALL.Length)];
                }
                return r;
            }
        }

        public SKColor[] ALL = new SKColor[] { };
        public  SKColor LightPink = new SKColor(255, 182, 193);//#FFB6C1 浅粉红
        public  SKColor Pink = new SKColor(255, 192, 203);//#FFC0CB 粉红
        public  SKColor Crimson = new SKColor(220, 20, 60);//#DC143C 深红(猩红)
        public  SKColor LavenderBlush = new SKColor(255, 240, 245);//#FFF0F5 淡紫红
        public  SKColor PaleVioletRed = new SKColor(219, 112, 147);//#DB7093 弱紫罗兰红
        public  SKColor HotPink = new SKColor(255, 105, 180);//#FF69B4 热情的粉红
        public  SKColor DeepPink = new SKColor(255, 20, 147);//#FF1493 深粉红
        public  SKColor MediumVioletRed = new SKColor(199, 21, 133);//#C71585 中紫罗兰红
        public  SKColor Orchid = new SKColor(218, 112, 214);//#DA70D6 暗紫色(兰花紫)
        public  SKColor Thistle = new SKColor(216, 191, 216);//#D8BFD8 蓟色
        public  SKColor Plum = new SKColor(221, 160, 221);//#DDA0DD 洋李色(李子紫)
        public  SKColor Violet = new SKColor(238, 130, 238);//#EE82EE 紫罗兰
        public  SKColor Magenta = new SKColor(255, 0, 255);//#FF00FF 洋红(玫瑰红)
        public  SKColor Fuchsia = new SKColor(255, 0, 255);//#FF00FF 紫红(灯笼海棠)
        public  SKColor DarkMagenta = new SKColor(139, 0, 139);//#8B008B 深洋红
        public  SKColor Purple = new SKColor(128, 0, 128);//#800080 紫色
        public  SKColor MediumOrchid = new SKColor(186, 85, 211);//#BA55D3 中兰花紫
        public  SKColor DarkViolet = new SKColor(148, 0, 211);//#9400D3 暗紫罗兰
        public  SKColor DarkOrchid = new SKColor(153, 50, 204);//#9932CC 暗兰花紫
        public  SKColor Indigo = new SKColor(75, 0, 130);//#4B0082 靛青/紫兰色
        public  SKColor BlueViolet = new SKColor(138, 43, 226);//#8A2BE2 蓝紫罗兰
        public  SKColor MediumPurple = new SKColor(147, 112, 219);//#9370DB 中紫色
        public  SKColor MediumSlateBlue = new SKColor(123, 104, 238);//#7B68EE 中暗蓝色(中板岩蓝)
        public  SKColor SlateBlue = new SKColor(106, 90, 205);//#6A5ACD 石蓝色(板岩蓝)
        public  SKColor DarkSlateBlue = new SKColor(72, 61, 139);//#483D8B 暗灰蓝色(暗板岩蓝)
        public  SKColor Lavender = new SKColor(230, 230, 250);//#E6E6FA 淡紫色(熏衣草淡紫)
        public  SKColor GhostWhite = new SKColor(248, 248, 255);//#F8F8FF 幽灵白
        public  SKColor Blue = new SKColor(0, 0, 255);//#0000FF 纯蓝
        public  SKColor MediumBlue = new SKColor(0, 0, 205);//#0000CD 中蓝色
        public  SKColor MidnightBlue = new SKColor(25, 25, 112);//#191970 午夜蓝
        public  SKColor DarkBlue = new SKColor(0, 0, 139);//#00008B 暗蓝色
        public  SKColor Navy = new SKColor(0, 0, 128);//#000080 海军蓝
        public  SKColor RoyalBlue = new SKColor(65, 105, 225);//#4169E1 皇家蓝/宝蓝
        public  SKColor CornflowerBlue = new SKColor(100, 149, 237);//#6495ED 矢车菊蓝
        public  SKColor LightSteelBlue = new SKColor(176, 196, 222);//#B0C4DE 亮钢蓝
        public  SKColor LightSlateGray = new SKColor(119, 136, 153);//#778899 亮蓝灰(亮石板灰)
        public  SKColor SlateGray = new SKColor(112, 128, 144);//#708090 灰石色(石板灰)
        public  SKColor DodgerBlue = new SKColor(30, 144, 255);//#1E90FF 闪兰色(道奇蓝)
        public  SKColor AliceBlue = new SKColor(240, 248, 255);//#F0F8FF 爱丽丝蓝
        public  SKColor SteelBlue = new SKColor(70, 130, 180);//#4682B4 钢蓝/铁青
        public  SKColor LightSkyBlue = new SKColor(135, 206, 250);//#87CEFA 亮天蓝色
        public  SKColor SkyBlue = new SKColor(135, 206, 235);//#87CEEB 天蓝色
        public  SKColor DeepSkyBlue = new SKColor(0, 191, 255);//#00BFFF 深天蓝
        public  SKColor LightBlue = new SKColor(173, 216, 230);//#ADD8E6 亮蓝
        public  SKColor PowderBlue = new SKColor(176, 224, 230);//#B0E0E6 粉蓝色(火药青)
        public  SKColor CadetBlue = new SKColor(95, 158, 160);//#5F9EA0 军兰色(军服蓝)
        public  SKColor Azure = new SKColor(240, 255, 255);//#F0FFFF 蔚蓝色
        public  SKColor LightCyan = new SKColor(224, 255, 255);//#E0FFFF 淡青色
        public  SKColor PaleTurquoise = new SKColor(175, 238, 238);//#AFEEEE 弱绿宝石
        public  SKColor Cyan = new SKColor(0, 255, 255);//#00FFFF 青色
        public  SKColor Aqua = new SKColor(0, 255, 255);//#00FFFF 浅绿色(水色)
        public  SKColor DarkTurquoise = new SKColor(0, 206, 209);//#00CED1 暗绿宝石
        public  SKColor DarkSlateGray = new SKColor(47, 79, 79);//#2F4F4F 暗瓦灰色(暗石板灰)
        public  SKColor DarkCyan = new SKColor(0, 139, 139);//#008B8B 暗青色
        public  SKColor Teal = new SKColor(0, 128, 128);//#008080 水鸭色
        public  SKColor MediumTurquoise = new SKColor(72, 209, 204);//#48D1CC 中绿宝石
        public  SKColor LightSeaGreen = new SKColor(32, 178, 170);//#20B2AA 浅海洋绿
        public  SKColor Turquoise = new SKColor(64, 224, 208);//#40E0D0 绿宝石
        public  SKColor Aquamarine = new SKColor(127, 255, 212);//#7FFFD4 宝石碧绿
        public  SKColor MediumAquamarine = new SKColor(102, 205, 170);//#66CDAA 中宝石碧绿
        public  SKColor MediumSpringGreen = new SKColor(0, 250, 154);//#00FA9A 中春绿色
        public  SKColor MintCream = new SKColor(245, 255, 250);//#F5FFFA 薄荷奶油
        public  SKColor SpringGreen = new SKColor(0, 255, 127);//#00FF7F 春绿色
        public  SKColor MediumSeaGreen = new SKColor(60, 179, 113);//#3CB371 中海洋绿
        public  SKColor SeaGreen = new SKColor(46, 139, 87);//#2E8B57 海洋绿
        public  SKColor Honeydew = new SKColor(240, 255, 240);//#F0FFF0 蜜色(蜜瓜色)
        public  SKColor LightGreen = new SKColor(144, 238, 144);//#90EE90 淡绿色
        public  SKColor PaleGreen = new SKColor(152, 251, 152);//#98FB98 弱绿色
        public  SKColor DarkSeaGreen = new SKColor(143, 188, 143);//#8FBC8F 暗海洋绿
        public  SKColor LimeGreen = new SKColor(50, 205, 50);//#32CD32 闪光深绿
        public  SKColor Lime = new SKColor(0, 255, 0);//#00FF00 闪光绿
        public  SKColor ForestGreen = new SKColor(34, 139, 34);//#228B22 森林绿
        public  SKColor Green = new SKColor(0, 128, 0);//#008000 纯绿
        public  SKColor DarkGreen = new SKColor(0, 100, 0);//#006400 暗绿色
        public  SKColor Chartreuse = new SKColor(127, 255, 0);//#7FFF00 黄绿色(查特酒绿)
        public  SKColor LawnGreen = new SKColor(124, 252, 0);//#7CFC00 草绿色(草坪绿_
        public  SKColor GreenYellow = new SKColor(173, 255, 47);//#ADFF2F 绿黄色
        public  SKColor DarkOliveGreen = new SKColor(85, 107, 47);//#556B2F 暗橄榄绿
        public  SKColor YellowGreen = new SKColor(154, 205, 50);//#9ACD32 黄绿色
        public  SKColor OliveDrab = new SKColor(107, 142, 35);//#6B8E23 橄榄褐色
        public  SKColor Beige = new SKColor(245, 245, 220);//#F5F5DC 米色/灰棕色
        public  SKColor LightGoldenrodYellow = new SKColor(250, 250, 210);//#FAFAD2 亮菊黄
        public  SKColor Ivory = new SKColor(255, 255, 240);//#FFFFF0 象牙色
        public  SKColor LightYellow = new SKColor(255, 255, 224);//#FFFFE0 浅黄色
        public  SKColor Yellow = new SKColor(255, 255, 0);//#FFFF00 纯黄
        public  SKColor Olive = new SKColor(128, 128, 0);//#808000 橄榄
        public  SKColor DarkKhaki = new SKColor(189, 183, 107);//#BDB76B 暗黄褐色(深卡叽布)
        public  SKColor LemonChiffon = new SKColor(255, 250, 205);//#FFFACD 柠檬绸
        public  SKColor PaleGoldenrod = new SKColor(238, 232, 170);//#EEE8AA 灰菊黄(苍麒麟色)
        public  SKColor Khaki = new SKColor(240, 230, 140);//#F0E68C 黄褐色(卡叽布)
        public  SKColor Gold = new SKColor(255, 215, 0);//#FFD700 金色
        public  SKColor Cornsilk = new SKColor(255, 248, 220);//#FFF8DC 玉米丝色
        public  SKColor Goldenrod = new SKColor(218, 165, 32);//#DAA520 金菊黄
        public  SKColor DarkGoldenrod = new SKColor(184, 134, 11);//#B8860B 暗金菊黄
        public  SKColor FloralWhite = new SKColor(255, 250, 240);//#FFFAF0 花的白色
        public  SKColor OldLace = new SKColor(253, 245, 230);//#FDF5E6 老花色(旧蕾丝)
        public  SKColor Wheat = new SKColor(245, 222, 179);//#F5DEB3 浅黄色(小麦色)
        public  SKColor Moccasin = new SKColor(255, 228, 181);//#FFE4B5 鹿皮色(鹿皮靴)
        public  SKColor Orange = new SKColor(255, 165, 0);//#FFA500 橙色
        public  SKColor PapayaWhip = new SKColor(255, 239, 213);//#FFEFD5 番木色(番木瓜)
        public  SKColor BlanchedAlmond = new SKColor(255, 235, 205);//#FFEBCD 白杏色
        public  SKColor NavajoWhite = new SKColor(255, 222, 173);//#FFDEAD 纳瓦白(土著白)
        public  SKColor AntiqueWhite = new SKColor(250, 235, 215);//#FAEBD7 古董白
        public  SKColor Tan = new SKColor(210, 180, 140);//#D2B48C 茶色
        public  SKColor BurlyWood = new SKColor(222, 184, 135);//#DEB887 硬木色
        public  SKColor Bisque = new SKColor(255, 228, 196);//#FFE4C4 陶坯黄
        public  SKColor DarkOrange = new SKColor(255, 140, 0);//#FF8C00 深橙色
        public  SKColor Linen = new SKColor(250, 240, 230);//#FAF0E6 亚麻布
        public  SKColor Peru = new SKColor(205, 133, 63);//#CD853F 秘鲁色
        public  SKColor PeachPuff = new SKColor(255, 218, 185);//#FFDAB9 桃肉色
        public  SKColor SandyBrown = new SKColor(244, 164, 96);//#F4A460 沙棕色
        public  SKColor Chocolate = new SKColor(210, 105, 30);//#D2691E 巧克力色
        public  SKColor SaddleBrown = new SKColor(139, 69, 19);//#8B4513 重褐色(马鞍棕色)
        public  SKColor Seashell = new SKColor(255, 245, 238);//#FFF5EE 海贝壳
        public  SKColor Sienna = new SKColor(160, 82, 45);//#A0522D 黄土赭色
        public  SKColor LightSalmon = new SKColor(255, 160, 122);//#FFA07A 浅鲑鱼肉色
        public  SKColor Coral = new SKColor(255, 127, 80);//#FF7F50 珊瑚
        public  SKColor OrangeRed = new SKColor(255, 69, 0);//#FF4500 橙红色
        public  SKColor DarkSalmon = new SKColor(233, 150, 122);//#E9967A 深鲜肉/鲑鱼色
        public  SKColor Tomato = new SKColor(255, 99, 71);//#FF6347 番茄红
        public  SKColor MistyRose = new SKColor(255, 228, 225);//#FFE4E1 浅玫瑰色(薄雾玫瑰)
        public  SKColor Salmon = new SKColor(250, 128, 114);//#FA8072 鲜肉/鲑鱼色
        public  SKColor Snow = new SKColor(255, 250, 250);//#FFFAFA 雪白色
        public  SKColor LightCoral = new SKColor(240, 128, 128);//#F08080 淡珊瑚色
        public  SKColor RosyBrown = new SKColor(188, 143, 143);//#BC8F8F 玫瑰棕色
        public  SKColor IndianRed = new SKColor(205, 92, 92);//#CD5C5C 印度红
        public  SKColor Red = new SKColor(255, 0, 0);//#FF0000 纯红
        public  SKColor Brown = new SKColor(165, 42, 42);//#A52A2A 棕色
        public  SKColor FireBrick = new SKColor(178, 34, 34);//#B22222 火砖色(耐火砖)
        public  SKColor DarkRed = new SKColor(139, 0, 0);//#8B0000 深红色
        public  SKColor Maroon = new SKColor(128, 0, 0);//#800000 栗色
        public  SKColor White = new SKColor(255, 255, 255);//#FFFFFF 纯白
        public  SKColor WhiteSmoke = new SKColor(245, 245, 245);//#F5F5F5 白烟
        public  SKColor Gainsboro = new SKColor(220, 220, 220);//#DCDCDC 淡灰色(庚斯博罗灰)
        public  SKColor LightGrey = new SKColor(211, 211, 211);//#D3D3D3 浅灰色
        public  SKColor Silver = new SKColor(192, 192, 192);//#C0C0C0 银灰色
        public  SKColor DarkGray = new SKColor(169, 169, 169);//#A9A9A9 深灰色
        public  SKColor Gray = new SKColor(128, 128, 128);//#808080 灰色
        public  SKColor DimGray = new SKColor(105, 105, 105);//#696969 暗淡的灰色
        public  SKColor Black = new SKColor(0, 0, 0);//#000000 纯黑

    }
}
