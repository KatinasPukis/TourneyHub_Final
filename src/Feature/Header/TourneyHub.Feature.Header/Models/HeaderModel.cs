﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TourneyHub.Feature.Registration.Models;

namespace TourneyHub.Feature.Header.Models
{
    public class HeaderModel
    {
        public string LogoID { get; set; }
        public MenuViewModel Page { get; set; }
        public string LinkToUserPage { get; set; }
    }
}