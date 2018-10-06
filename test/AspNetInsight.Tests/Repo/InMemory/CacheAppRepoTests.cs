using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using AspNetInsight;
using AspNetInsight.Dto;
using AspNetInsight.Repo;

namespace AspNetInsight.Tests.Repo.InMemory
{
    /// <summary>
    /// CacheAppRepo unit tests
    /// </summary>
    public class CacheAppRepoTests
    {
        public virtual IAppRepo NewApp() => new CacheAppRepo();

        [Fact]
        public void New()
        {
            // arrange
            var n = NewApp();

            // act
            var app = n.New(new App() { AppId = "1-1", AppName = "n1-1", MachineName = "mn", Url = "url1-1" });
            var app1 = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = "mn", Url = "url1-2" });

            // assert
            Assert.NotNull(app);
            Assert.NotNull(app1);
            Assert.NotSame(app, app1);
            Assert.NotEqual<long>(app.Id, app1.Id);
            Assert.True(app.Id > 0);
        }

        [Fact]
        public void New_with_invalid_argument()
        {
            // arrange
            var n = NewApp();

            // act and assert
            Assert.Throws<ArgumentNullException>(() => n.New(null));
        }

        [Fact]
        public void GetApp_with_valid_id()
        {
            // arrange
            var n = NewApp();
            var app = n.New(new App() { AppId = "1-1", AppName = "n1-1", MachineName = "mn", Url = "url1-1" });
            var app1 = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = "mn", Url = "url1-2" });
            var app2 = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = "mn", Url = "url1-2" });

            // act
            var gapp = n.GetApp(app.Id);
            
            // assert
            Assert.NotNull(gapp);
            Assert.NotNull(app);
            Assert.NotSame(gapp, app);
            Assert.Equal(gapp.Id, app.Id);
            Assert.True(gapp.AppId == app.AppId);
            Assert.True(gapp.AppName == app.AppName);
            Assert.True(gapp.MachineName == app.MachineName);
            Assert.True(gapp.Url == app.Url);
        }

        [Fact]
        public void GetApp_with_invalid_id()
        {
            // arrange
            var n = NewApp();
            
            // act
            var gapp = n.GetApp(0);

            // assert
            Assert.Null(gapp);
        }

        [Fact]
        public void GetAppById_with_invalid_id()
        {
            // arrange
            var n = NewApp();

            // act
            var gapp = n.GetAppById(Guid.NewGuid().ToString());

            // assert
            Assert.Null(gapp);
        }

        [Fact]
        public void GetAppById_with_valid_id()
        {
            // arrange
            var n = NewApp();
            var app = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = "mn", Url = "url1-2" });

            // act
            var gapp = n.GetAppById(app.AppId);

            // assert
            Assert.NotNull(gapp);
            Assert.NotSame(gapp, app);
            Assert.True(gapp.AppId == app.AppId);
        }

        [Fact]
        public void GetAppById_with_exception()
        {
            // arrange
            var n = NewApp();
            var app = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = "mn", Url = "url1-2" });

            // act and assert
            Assert.Throws<ArgumentNullException>(() => n.GetAppById(null));
        }

        //GetAppByMachineName
        [Fact]
        public void GetAppByMachineName_with_valid_id()
        {
            // arrange
            var n = NewApp();
            var name = "machine-name-1";
            var app = n.New(new App() { AppId = "1-1", AppName = "n1-1", MachineName = name, Url = "url1-1" });
            var app1 = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = name, Url = "url1-2" });
            var app2 = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = name, Url = "url1-2" });

            // act
            var apps = n.GetAppByMachineName(name);

            // assert
            Assert.True(apps.Count() > 0);
            Assert.NotEmpty(apps);
            foreach(var a in apps)
            {
                Assert.True(a.MachineName == name);
            }
        }

        [Fact]
        public void GetAppByMachineName_with_invalid_machine()
        {
            // arrange
            var n = NewApp();
            var app = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = "mn", Url = "url1-2" });

            // act and assert
            Assert.Throws<ArgumentNullException>(() => n.GetAppByMachineName(null));
        }

        [Fact]
        public void GetAppByUrl_with_invalid_url()
        {
            // arrange
            var n = NewApp();
            var app = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = "mn", Url = "url1-2" });

            // act and assert
            Assert.Throws<ArgumentNullException>(() => n.GetAppByUrl(null));
        }

        [Fact]
        public void GetAppByUrl_with_valid_url()
        {
            // arrange
            var n = NewApp();
            var name = "machine-name-1";
            var url = "https://localhost:8765/";
            var app = n.New(new App() { AppId = "1-1", AppName = "n1-1", MachineName = name, Url = "http://localhost:8765/" });
            var app1 = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = name, Url = url });
            var app2 = n.New(new App() { AppId = "1-2", AppName = "n1-2", MachineName = name, Url = url });

            // act
            var apps = n.GetAppByUrl(url);
            var noapp = n.GetAppByUrl(Guid.NewGuid().ToString());

            // assert
            Assert.Null(noapp);
            Assert.True(apps.Count() > 0);
            Assert.NotEmpty(apps);
            foreach (var a in apps)
            {
                Assert.True(a.Url == url);
            }

        }
    }
}
