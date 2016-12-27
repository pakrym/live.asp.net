// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using live.asp.net.Models;
using live.asp.net.Services;
using live.asp.net.ViewModels;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace live.asp.net.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILiveShowDetailsService _liveShowDetails;
        private readonly TelemetryClient _client;
        private readonly IShowsService _showsService;

        public HomeController(IShowsService showsService, ILiveShowDetailsService liveShowDetails, TelemetryClient client)
        {
            _showsService = showsService;
            _liveShowDetails = liveShowDetails;
            _client = client;
        }

        [Route("/")]
        public async Task<IActionResult> Index(bool? disableCache)
        {
            _client.TrackDependency("HomeController.Index", "Index", DateTimeOffset.Now, TimeSpan.FromMilliseconds(100), true);
            _client.TrackDependency("Other", "Whatever", "NONSQL", "Index", DateTimeOffset.Now, TimeSpan.FromMilliseconds(100), "0", true);

            var liveShowDetails = await _liveShowDetails.LoadAsync();
            var showList = await _showsService.GetRecordedShowsAsync(User, disableCache ?? false);

            return View(new HomeViewModel
            {
                AdminMessage = liveShowDetails?.AdminMessage,
                NextShowDateUtc = liveShowDetails?.NextShowDateUtc,
                LiveShowEmbedUrl = liveShowDetails?.LiveShowEmbedUrl,
                LiveShowHtml = liveShowDetails?.LiveShowHtml,
                PreviousShows = showList.Shows,
                MoreShowsUrl = showList.MoreShowsUrl
            });
        }

        [HttpGet("/ical")]
        [Produces("text/calendar")]
        public async Task<LiveShowDetails> GetiCal()
        {
            var liveShowDetails = await _liveShowDetails.LoadAsync();

            return liveShowDetails;
        }

        [Route("/error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
