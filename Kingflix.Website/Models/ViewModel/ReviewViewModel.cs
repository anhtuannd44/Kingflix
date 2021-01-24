using System.Collections.Generic;

namespace Kingflix.Models
{
    public class RevieViewModel
    {
        public double StarAvg { get; set; }
        public List<EachStarViewModel> Data = new List<EachStarViewModel>();
    }
    public class EachStarViewModel
    {
        public int StarType { get; set; }
        public int ReviewCount { get; set; }
        public double ReviewPercent { get; set; }
    }

}