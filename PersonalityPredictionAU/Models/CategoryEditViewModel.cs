using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalityPredictionAU.Models
{
    public class CategoryEditViewModel
    {
        public Category Category { get; set; }
        public IEnumerable<Word> Words { get; set; }
    }
}