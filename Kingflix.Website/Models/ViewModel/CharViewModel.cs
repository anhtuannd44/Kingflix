using Kingflix.Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kingflix.Models.ViewModel
{
    public class ChartViewModel
    {
        public string[] labels { get; set; }
        public List<Datasets> datasets = new List<Datasets>();
    }

    public class Datasets
    {
        public string label { get; set; }
        public double[] data { get;set; }
        public string yAxisID { get; set; }
        public string[] backgroundColor { get; set; }
        public string[] borderColor { get; set; }
        public int borderWidth
        {
            get
            {
                return 1;
            }
        }
        public bool fill
        {
            get
            {
                return false;
            }
        }
    }

}