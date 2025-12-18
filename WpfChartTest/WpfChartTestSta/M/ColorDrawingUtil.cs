using System;
using System.Drawing;

namespace MKSS.APP.ECTester.UserCommon
{


    public class ColorDrawingUtil
    {
        static ColorDrawingUtil of = new ColorDrawingUtil() {
            
        };
        public static ColorDrawingUtil Of
        {
            get {
                return of;
            }
        }

        public ColorDrawingUtil() {
            ALL = new Color[] {
                LightPink,Pink,Crimson,LavenderBlush,PaleVioletRed,HotPink,DeepPink,MediumVioletRed,Orchid,Thistle,Plum,Violet,Magenta,Fuchsia,DarkMagenta,Purple,MediumOrchid,DarkViolet,DarkOrchid,Indigo,BlueViolet,MediumPurple,MediumSlateBlue,SlateBlue,DarkSlateBlue,Lavender,GhostWhite,Blue,MediumBlue,MidnightBlue,DarkBlue,Navy,RoyalBlue,CornflowerBlue,LightSteelBlue,LightSlateGray,SlateGray,DodgerBlue,AliceBlue,SteelBlue,LightSkyBlue,SkyBlue,DeepSkyBlue,LightBlue,PowderBlue,CadetBlue,Azure,LightCyan,PaleTurquoise,Cyan,Aqua,DarkTurquoise,DarkSlateGray,DarkCyan,Teal,MediumTurquoise,LightSeaGreen,Turquoise,Aquamarine,MediumAquamarine,MediumSpringGreen,MintCream,SpringGreen,MediumSeaGreen,SeaGreen,Honeydew,LightGreen,PaleGreen,DarkSeaGreen,LimeGreen,Lime,ForestGreen,Green,DarkGreen,Chartreuse,LawnGreen,GreenYellow,DarkOliveGreen,YellowGreen,OliveDrab,Beige,LightGoldenrodYellow,Ivory,LightYellow,Yellow,Olive,DarkKhaki,LemonChiffon,PaleGoldenrod,Khaki,Gold,Cornsilk,Goldenrod,DarkGoldenrod,FloralWhite,OldLace,Wheat,Moccasin,Orange,PapayaWhip,BlanchedAlmond,NavajoWhite,AntiqueWhite,Tan,BurlyWood,Bisque,DarkOrange,Linen,Peru,PeachPuff,SandyBrown,Chocolate,SaddleBrown,Seashell,Sienna,LightSalmon,Coral,OrangeRed,DarkSalmon,Tomato,MistyRose,Salmon,Snow,LightCoral,RosyBrown,IndianRed,Red,Brown,FireBrick,DarkRed,Maroon,White,WhiteSmoke,Gainsboro,LightGrey,Silver,DarkGray,Gray,DimGray,Black
            };
        } 

        /// <summary>
        /// 随机神色
        /// </summary>
        public Color RandomDark {
            get {
                byte[] buffer = Guid.NewGuid().ToByteArray();
                int iSeed = BitConverter.ToInt32(buffer, 0);
                Random random = new Random(iSeed);
                Color r = ALL[random.Next(0, ALL.Length)];
                while (r.R + r.G + r.B > 450) {
                    r = ALL[random.Next(0, ALL.Length)];
                }
                return r;
            }
        }

        /// <summary>
        /// 随机神色
        /// </summary>
        public Color RandomNoDark
        {
            get
            {
                byte[] buffer = Guid.NewGuid().ToByteArray();
                int iSeed = BitConverter.ToInt32(buffer, 0);
                Random random = new Random(iSeed);
                Color r = ALL[random.Next(0, ALL.Length)];
                while (r.R + r.G + r.B < 100)
                {
                    r = ALL[random.Next(0, ALL.Length)];
                }
                return r;
            }
        }


        /// <summary>
        /// 随机神色
        /// </summary>
        public Color RandomNoDarkAlpha(byte alpha)
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            Random random = new Random(iSeed);
            Color r = ALL[random.Next(0, ALL.Length)];
            while (r.R + r.G + r.B < 100)
            {
                r = ALL[random.Next(0, ALL.Length)];
            }
            return Color.FromArgb(alpha, r.R,r.G,r.B);
        }

        public Color[] ALL = new Color[] { };
        public  Color LightPink = Color.FromArgb(255,255, 182, 193);//#FFB6C1 浅粉红
        public  Color Pink = Color.FromArgb(255,255, 192, 203);//#FFC0CB 粉红
        public  Color Crimson = Color.FromArgb(255,220, 20, 60);//#DC143C 深红(猩红)
        public  Color LavenderBlush = Color.FromArgb(255,255, 240, 245);//#FFF0F5 淡紫红
        public  Color PaleVioletRed = Color.FromArgb(255,219, 112, 147);//#DB7093 弱紫罗兰红
        public  Color HotPink = Color.FromArgb(255,255, 105, 180);//#FF69B4 热情的粉红
        public  Color DeepPink = Color.FromArgb(255,255, 20, 147);//#FF1493 深粉红
        public  Color MediumVioletRed = Color.FromArgb(255,199, 21, 133);//#C71585 中紫罗兰红
        public  Color Orchid = Color.FromArgb(255,218, 112, 214);//#DA70D6 暗紫色(兰花紫)
        public  Color Thistle = Color.FromArgb(255,216, 191, 216);//#D8BFD8 蓟色
        public  Color Plum = Color.FromArgb(255,221, 160, 221);//#DDA0DD 洋李色(李子紫)
        public  Color Violet = Color.FromArgb(255,238, 130, 238);//#EE82EE 紫罗兰
        public  Color Magenta = Color.FromArgb(255,255, 0, 255);//#FF00FF 洋红(玫瑰红)
        public  Color Fuchsia = Color.FromArgb(255,255, 0, 255);//#FF00FF 紫红(灯笼海棠)
        public  Color DarkMagenta = Color.FromArgb(255,139, 0, 139);//#8B008B 深洋红
        public  Color Purple = Color.FromArgb(255,128, 0, 128);//#800080 紫色
        public  Color MediumOrchid = Color.FromArgb(255,186, 85, 211);//#BA55D3 中兰花紫
        public  Color DarkViolet = Color.FromArgb(255,148, 0, 211);//#9400D3 暗紫罗兰
        public  Color DarkOrchid = Color.FromArgb(255,153, 50, 204);//#9932CC 暗兰花紫
        public  Color Indigo = Color.FromArgb(255,75, 0, 130);//#4B0082 靛青/紫兰色
        public  Color BlueViolet = Color.FromArgb(255,138, 43, 226);//#8A2BE2 蓝紫罗兰
        public  Color MediumPurple = Color.FromArgb(255,147, 112, 219);//#9370DB 中紫色
        public  Color MediumSlateBlue = Color.FromArgb(255,123, 104, 238);//#7B68EE 中暗蓝色(中板岩蓝)
        public  Color SlateBlue = Color.FromArgb(255,106, 90, 205);//#6A5ACD 石蓝色(板岩蓝)
        public  Color DarkSlateBlue = Color.FromArgb(255,72, 61, 139);//#483D8B 暗灰蓝色(暗板岩蓝)
        public  Color Lavender = Color.FromArgb(255,230, 230, 250);//#E6E6FA 淡紫色(熏衣草淡紫)
        public  Color GhostWhite = Color.FromArgb(255,248, 248, 255);//#F8F8FF 幽灵白
        public  Color Blue = Color.FromArgb(255,0, 0, 255);//#0000FF 纯蓝
        public  Color MediumBlue = Color.FromArgb(255,0, 0, 205);//#0000CD 中蓝色
        public  Color MidnightBlue = Color.FromArgb(255,25, 25, 112);//#191970 午夜蓝
        public  Color DarkBlue = Color.FromArgb(255,0, 0, 139);//#00008B 暗蓝色
        public  Color Navy = Color.FromArgb(255,0, 0, 128);//#000080 海军蓝
        public  Color RoyalBlue = Color.FromArgb(255,65, 105, 225);//#4169E1 皇家蓝/宝蓝
        public  Color CornflowerBlue = Color.FromArgb(255,100, 149, 237);//#6495ED 矢车菊蓝
        public  Color LightSteelBlue = Color.FromArgb(255,176, 196, 222);//#B0C4DE 亮钢蓝
        public  Color LightSlateGray = Color.FromArgb(255,119, 136, 153);//#778899 亮蓝灰(亮石板灰)
        public  Color SlateGray = Color.FromArgb(255,112, 128, 144);//#708090 灰石色(石板灰)
        public  Color DodgerBlue = Color.FromArgb(255,30, 144, 255);//#1E90FF 闪兰色(道奇蓝)
        public  Color AliceBlue = Color.FromArgb(255,240, 248, 255);//#F0F8FF 爱丽丝蓝
        public  Color SteelBlue = Color.FromArgb(255,70, 130, 180);//#4682B4 钢蓝/铁青
        public  Color LightSkyBlue = Color.FromArgb(255,135, 206, 250);//#87CEFA 亮天蓝色
        public  Color SkyBlue = Color.FromArgb(255,135, 206, 235);//#87CEEB 天蓝色
        public  Color DeepSkyBlue = Color.FromArgb(255,0, 191, 255);//#00BFFF 深天蓝
        public  Color LightBlue = Color.FromArgb(255,173, 216, 230);//#ADD8E6 亮蓝
        public  Color PowderBlue = Color.FromArgb(255,176, 224, 230);//#B0E0E6 粉蓝色(火药青)
        public  Color CadetBlue = Color.FromArgb(255,95, 158, 160);//#5F9EA0 军兰色(军服蓝)
        public  Color Azure = Color.FromArgb(255,240, 255, 255);//#F0FFFF 蔚蓝色
        public  Color LightCyan = Color.FromArgb(255,224, 255, 255);//#E0FFFF 淡青色
        public  Color PaleTurquoise = Color.FromArgb(255,175, 238, 238);//#AFEEEE 弱绿宝石
        public  Color Cyan = Color.FromArgb(255,0, 255, 255);//#00FFFF 青色
        public  Color Aqua = Color.FromArgb(255,0, 255, 255);//#00FFFF 浅绿色(水色)
        public  Color DarkTurquoise = Color.FromArgb(255,0, 206, 209);//#00CED1 暗绿宝石
        public  Color DarkSlateGray = Color.FromArgb(255,47, 79, 79);//#2F4F4F 暗瓦灰色(暗石板灰)
        public  Color DarkCyan = Color.FromArgb(255,0, 139, 139);//#008B8B 暗青色
        public  Color Teal = Color.FromArgb(255,0, 128, 128);//#008080 水鸭色
        public  Color MediumTurquoise = Color.FromArgb(255,72, 209, 204);//#48D1CC 中绿宝石
        public  Color LightSeaGreen = Color.FromArgb(255,32, 178, 170);//#20B2AA 浅海洋绿
        public  Color Turquoise = Color.FromArgb(255,64, 224, 208);//#40E0D0 绿宝石
        public  Color Aquamarine = Color.FromArgb(255,127, 255, 212);//#7FFFD4 宝石碧绿
        public  Color MediumAquamarine = Color.FromArgb(255,102, 205, 170);//#66CDAA 中宝石碧绿
        public  Color MediumSpringGreen = Color.FromArgb(255,0, 250, 154);//#00FA9A 中春绿色
        public  Color MintCream = Color.FromArgb(255,245, 255, 250);//#F5FFFA 薄荷奶油
        public  Color SpringGreen = Color.FromArgb(255,0, 255, 127);//#00FF7F 春绿色
        public  Color MediumSeaGreen = Color.FromArgb(255,60, 179, 113);//#3CB371 中海洋绿
        public  Color SeaGreen = Color.FromArgb(255,46, 139, 87);//#2E8B57 海洋绿
        public  Color Honeydew = Color.FromArgb(255,240, 255, 240);//#F0FFF0 蜜色(蜜瓜色)
        public  Color LightGreen = Color.FromArgb(255,144, 238, 144);//#90EE90 淡绿色
        public  Color PaleGreen = Color.FromArgb(255,152, 251, 152);//#98FB98 弱绿色
        public  Color DarkSeaGreen = Color.FromArgb(255,143, 188, 143);//#8FBC8F 暗海洋绿
        public  Color LimeGreen = Color.FromArgb(255,50, 205, 50);//#32CD32 闪光深绿
        public  Color Lime = Color.FromArgb(255,0, 255, 0);//#00FF00 闪光绿
        public  Color ForestGreen = Color.FromArgb(255,34, 139, 34);//#228B22 森林绿
        public  Color Green = Color.FromArgb(255,0, 128, 0);//#008000 纯绿
        public  Color DarkGreen = Color.FromArgb(255,0, 100, 0);//#006400 暗绿色
        public  Color Chartreuse = Color.FromArgb(255,127, 255, 0);//#7FFF00 黄绿色(查特酒绿)
        public  Color LawnGreen = Color.FromArgb(255,124, 252, 0);//#7CFC00 草绿色(草坪绿_
        public  Color GreenYellow = Color.FromArgb(255,173, 255, 47);//#ADFF2F 绿黄色
        public  Color DarkOliveGreen = Color.FromArgb(255,85, 107, 47);//#556B2F 暗橄榄绿
        public  Color YellowGreen = Color.FromArgb(255,154, 205, 50);//#9ACD32 黄绿色
        public  Color OliveDrab = Color.FromArgb(255,107, 142, 35);//#6B8E23 橄榄褐色
        public  Color Beige = Color.FromArgb(255,245, 245, 220);//#F5F5DC 米色/灰棕色
        public  Color LightGoldenrodYellow = Color.FromArgb(255,250, 250, 210);//#FAFAD2 亮菊黄
        public  Color Ivory = Color.FromArgb(255,255, 255, 240);//#FFFFF0 象牙色
        public  Color LightYellow = Color.FromArgb(255,255, 255, 224);//#FFFFE0 浅黄色
        public  Color Yellow = Color.FromArgb(255,255, 255, 0);//#FFFF00 纯黄
        public  Color Olive = Color.FromArgb(255,128, 128, 0);//#808000 橄榄
        public  Color DarkKhaki = Color.FromArgb(255,189, 183, 107);//#BDB76B 暗黄褐色(深卡叽布)
        public  Color LemonChiffon = Color.FromArgb(255,255, 250, 205);//#FFFACD 柠檬绸
        public  Color PaleGoldenrod = Color.FromArgb(255,238, 232, 170);//#EEE8AA 灰菊黄(苍麒麟色)
        public  Color Khaki = Color.FromArgb(255,240, 230, 140);//#F0E68C 黄褐色(卡叽布)
        public  Color Gold = Color.FromArgb(255,255, 215, 0);//#FFD700 金色
        public  Color Cornsilk = Color.FromArgb(255,255, 248, 220);//#FFF8DC 玉米丝色
        public  Color Goldenrod = Color.FromArgb(255,218, 165, 32);//#DAA520 金菊黄
        public  Color DarkGoldenrod = Color.FromArgb(255,184, 134, 11);//#B8860B 暗金菊黄
        public  Color FloralWhite = Color.FromArgb(255,255, 250, 240);//#FFFAF0 花的白色
        public  Color OldLace = Color.FromArgb(255,253, 245, 230);//#FDF5E6 老花色(旧蕾丝)
        public  Color Wheat = Color.FromArgb(255,245, 222, 179);//#F5DEB3 浅黄色(小麦色)
        public  Color Moccasin = Color.FromArgb(255,255, 228, 181);//#FFE4B5 鹿皮色(鹿皮靴)
        public  Color Orange = Color.FromArgb(255,255, 165, 0);//#FFA500 橙色
        public  Color PapayaWhip = Color.FromArgb(255,255, 239, 213);//#FFEFD5 番木色(番木瓜)
        public  Color BlanchedAlmond = Color.FromArgb(255,255, 235, 205);//#FFEBCD 白杏色
        public  Color NavajoWhite = Color.FromArgb(255,255, 222, 173);//#FFDEAD 纳瓦白(土著白)
        public  Color AntiqueWhite = Color.FromArgb(255,250, 235, 215);//#FAEBD7 古董白
        public  Color Tan = Color.FromArgb(255,210, 180, 140);//#D2B48C 茶色
        public  Color BurlyWood = Color.FromArgb(255,222, 184, 135);//#DEB887 硬木色
        public  Color Bisque = Color.FromArgb(255,255, 228, 196);//#FFE4C4 陶坯黄
        public  Color DarkOrange = Color.FromArgb(255,255, 140, 0);//#FF8C00 深橙色
        public  Color Linen = Color.FromArgb(255,250, 240, 230);//#FAF0E6 亚麻布
        public  Color Peru = Color.FromArgb(255,205, 133, 63);//#CD853F 秘鲁色
        public  Color PeachPuff = Color.FromArgb(255,255, 218, 185);//#FFDAB9 桃肉色
        public  Color SandyBrown = Color.FromArgb(255,244, 164, 96);//#F4A460 沙棕色
        public  Color Chocolate = Color.FromArgb(255,210, 105, 30);//#D2691E 巧克力色
        public  Color SaddleBrown = Color.FromArgb(255,139, 69, 19);//#8B4513 重褐色(马鞍棕色)
        public  Color Seashell = Color.FromArgb(255,255, 245, 238);//#FFF5EE 海贝壳
        public  Color Sienna = Color.FromArgb(255,160, 82, 45);//#A0522D 黄土赭色
        public  Color LightSalmon = Color.FromArgb(255,255, 160, 122);//#FFA07A 浅鲑鱼肉色
        public  Color Coral = Color.FromArgb(255,255, 127, 80);//#FF7F50 珊瑚
        public  Color OrangeRed = Color.FromArgb(255,255, 69, 0);//#FF4500 橙红色
        public  Color DarkSalmon = Color.FromArgb(255,233, 150, 122);//#E9967A 深鲜肉/鲑鱼色
        public  Color Tomato = Color.FromArgb(255,255, 99, 71);//#FF6347 番茄红
        public  Color MistyRose = Color.FromArgb(255,255, 228, 225);//#FFE4E1 浅玫瑰色(薄雾玫瑰)
        public  Color Salmon = Color.FromArgb(255,250, 128, 114);//#FA8072 鲜肉/鲑鱼色
        public  Color Snow = Color.FromArgb(255,255, 250, 250);//#FFFAFA 雪白色
        public  Color LightCoral = Color.FromArgb(255,240, 128, 128);//#F08080 淡珊瑚色
        public  Color RosyBrown = Color.FromArgb(255,188, 143, 143);//#BC8F8F 玫瑰棕色
        public  Color IndianRed = Color.FromArgb(255,205, 92, 92);//#CD5C5C 印度红
        public  Color Red = Color.FromArgb(255,255, 0, 0);//#FF0000 纯红
        public  Color Brown = Color.FromArgb(255,165, 42, 42);//#A52A2A 棕色
        public  Color FireBrick = Color.FromArgb(255,178, 34, 34);//#B22222 火砖色(耐火砖)
        public  Color DarkRed = Color.FromArgb(255,139, 0, 0);//#8B0000 深红色
        public  Color Maroon = Color.FromArgb(255,128, 0, 0);//#800000 栗色
        public  Color White = Color.FromArgb(255,255, 255, 255);//#FFFFFF 纯白
        public  Color WhiteSmoke = Color.FromArgb(255,245, 245, 245);//#F5F5F5 白烟
        public  Color Gainsboro = Color.FromArgb(255,220, 220, 220);//#DCDCDC 淡灰色(庚斯博罗灰)
        public  Color LightGrey = Color.FromArgb(255,211, 211, 211);//#D3D3D3 浅灰色
        public  Color Silver = Color.FromArgb(255,192, 192, 192);//#C0C0C0 银灰色
        public  Color DarkGray = Color.FromArgb(255,169, 169, 169);//#A9A9A9 深灰色
        public  Color Gray = Color.FromArgb(255,128, 128, 128);//#808080 灰色
        public  Color DimGray = Color.FromArgb(255,105, 105, 105);//#696969 暗淡的灰色
        public  Color Black = Color.FromArgb(255,0, 0, 0);//#000000 纯黑

    }
}
