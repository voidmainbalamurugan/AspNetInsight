using AspNetInsight.Dto;
using AspNetInsight.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AspNetInsight.Tests.Repo.InMemory
{
    /// <summary>
    /// CacheResponseLog unit test cases
    /// </summary>
    public class CacheResponseLogTests
    {
        public virtual ILogRepo<ResponseLog> NewLogRepo()
        {
            DefaultAppId = 123;
            return new CacheResponseLog();
        }
        public long DefaultAppId { get; protected set; }

        [Fact]
        public void Log_with_valid_data()
        {
            // arrange
            var log = NewLogRepo();
            var logentry = new ResponseLog()
            {
                AppId = DefaultAppId,
                ByteSent = 234,
                CreatedOn = DateTime.UtcNow,
                HanlderExeTime = 345,
                ResponseTime = 456,
                MachineName = "mname-1",
                RawUrl = "rurl-1",
                Url = "url-1", Scale = Size.KB, TimeScale = TimeSlice.Milliseconds 
            };

            // act and assert
            Assert.DoesNotThrow(() => log.Log(logentry));
        }

        [Fact]
        public void Log_with_invalid_data()
        {
            // arrange
            var log = NewLogRepo();

            // act // assert
            Assert.Throws<ArgumentNullException>(() => log.Log(null));
        }
        
        [Fact]
        public void GetByAppId_with_valid_id()
        {
            // arrange
            var log = NewLogRepo();
            var logentry = new ResponseLog()
            {
                AppId = DefaultAppId,
                ByteSent = 234,
                CreatedOn = DateTime.UtcNow,
                HanlderExeTime = 345,
                ResponseTime = 456,
                MachineName = "mname-1",
                RawUrl = "rurl-1",
                Url = "url-1",
                Scale = Size.KB,
                TimeScale = TimeSlice.Milliseconds
            };
            log.Log(logentry);

            // act 
            var actual = log.GetByAppId(DefaultAppId);

            // assert
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.True(actual.First().MachineName == "mname-1");
        }

        [Fact]
        public void GetByAppId_with_invalid_id()
        {
            // arrange
            var log = NewLogRepo();

            // act // assert
            var actual = log.GetByAppId(0);

            // assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetByAppId_for_valid_from_to()
        {
            // arrange
            var log = NewLogRepo();
            var logentry = new ResponseLog()
            {
                AppId = DefaultAppId,
                ByteSent = 234,
                CreatedOn = DateTime.Parse("2017-09-26"),
                HanlderExeTime = 345,
                ResponseTime = 456,
                MachineName = "mname-1",
                RawUrl = "rurl-1",
                Url = "url-1",
                Scale = Size.KB,
                TimeScale = TimeSlice.Milliseconds
            };
            log.Log(logentry);
            var rf = DateTime.Parse("2018-09-25");
            var another = logentry.DeepCopy();
            another.CreatedOn = rf;
            log.Log(another);

            // act 
            var actual = log.GetByAppId(DefaultAppId, rf.AddYears(-1), rf);

            // assert
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            foreach(var a in actual)
            {
                Assert.True(a.CreatedOn >= rf.AddYears(-1) && a.CreatedOn <= rf);
            }
        }

        [Fact]
        public void GetByAppId_for_valid_from_to_has_no_logentries()
        {
            // arrange
            var log = NewLogRepo();
            var logentry = new ResponseLog()
            {
                AppId = DefaultAppId,
                ByteSent = 234,
                CreatedOn = DateTime.Parse("2017-09-26"),
                HanlderExeTime = 345,
                ResponseTime = 456,
                MachineName = "mname-1",
                RawUrl = "rurl-1",
                Url = "url-1",
                Scale = Size.KB,
                TimeScale = TimeSlice.Milliseconds
            };
            log.Log(logentry);
            var rf = DateTime.Parse("2018-09-25");
            var another = logentry.DeepCopy();
            another.CreatedOn = rf;
            log.Log(another);

            // act 
            var actual = log.GetByAppId(DefaultAppId, rf.AddYears(10), rf.AddYears(11));

            // assert
            Assert.Null(actual);
        }
        
    }
}
