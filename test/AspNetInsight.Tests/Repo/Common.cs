using AspNetInsight.Dto;
using AspNetInsight.Repo;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetInsight.Tests.Repo
{
    class Common
    {
        public static IAppRepo GetMockAppRepo()
        {
            var mock = new Mock<IAppRepo>();
            var temp = new App() { Id = 123, AppId = "app-id", AppName = "app-name", MachineName = "mname", Url = "url" };
            mock.Setup(app => app.GetAppById(It.IsAny<string>())).Returns(temp);
            mock.Setup(app => app.GetApp(It.IsInRange<long>(1, long.MaxValue, Range.Inclusive))).Returns(temp);
            mock.Setup(app => app.GetAppByMachineName(It.IsAny<string>())).Returns(new List<App>() { temp });
            mock.Setup(app => app.GetAppByUrl(It.IsAny<string>())).Returns(new List<App>() { temp });

            return mock.Object;
        }
    }
}
