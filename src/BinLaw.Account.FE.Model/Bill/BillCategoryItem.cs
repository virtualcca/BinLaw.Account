using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace BinLaw.Account.FE.Model.Bill
{
    public class BillCategoryItem
    {
        private readonly ResourceLoader _resourceLoader = new ResourceLoader();
        private string _name;

        public int BillType { get; set; }

        public int Id { get; set; }

        public bool CanEdit { get; set; }

        public string Name { get { return _resourceLoader.GetString(_name); } set { _name = value; }}

        public string Icon { get; set; }

        public string BackGround { get; set; }

        private SolidColorBrush backGroundBrush;
        public SolidColorBrush BackGroundBrush { get { return backGroundBrush ?? (backGroundBrush = new SolidColorBrush(ToColor(BackGround))); } }

        public int CategoryId { get { return BillType*10000 + Id*100; } }

        private Color ToColor(string colorName)
        {
            if (colorName.StartsWith("#")) colorName = colorName.Replace("#", string.Empty);
            int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
            return new Color()
            {
                A = Convert.ToByte((v >> 24) & 255),
                R = Convert.ToByte((v >> 16) & 255),
                G = Convert.ToByte((v >> 8) & 255),
                B = Convert.ToByte((v >> 0) & 255)

            };
        }
    }

}
