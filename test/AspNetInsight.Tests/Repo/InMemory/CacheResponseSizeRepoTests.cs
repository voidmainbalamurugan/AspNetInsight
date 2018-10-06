using AspNetInsight.Dto;
using AspNetInsight.Repo;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AspNetInsight.Tests.Repo.InMemory
{
    /// <summary>
    /// CacheResponseSize unit test cases
    /// </summary>
    public class CacheResponseSizeRepoTests
    {
        public virtual IResponseRepo<AppResponseSize> NewResponseSizeRepo(IAppRepo repo = null)
        {
            DefaultAppId = 123;
            return new CacheResponseSizeRepo(repo);
        }
        public long DefaultAppId { get; protected set; }

        [Fact]
        public void AddAppData_for_valid()
        {
            // arrange
            var sizeRepo = NewResponseSizeRepo(Common.GetMockAppRepo());
            var data = new AppResponseSize()
            {
                AppId = DefaultAppId,
                ModifiedDate = DateTime.UtcNow,
                Recent = 10,
                Min = 5,
                Max = 20,
                Total = 6,
                Scale = Size.Byte
            };

            // act
            var actual = sizeRepo.AddAppData(data);

            // assert
            Assert.NotNull(actual);
            Assert.NotSame(actual, data);
            Assert.NotEqual(actual.Id, data.Id);
            Assert.True(actual.Id > 0);
            Assert.True(actual.AppId == data.AppId);
            Assert.True(actual.Recent == data.Recent);
            
        }

        [Fact]
        public void AddAppData_with_invalid_input()
        {
            // arrange

            var sizeRepo = NewResponseSizeRepo(Common.GetMockAppRepo());

            // act and assert
            Assert.Throws<ArgumentNullException>(() => sizeRepo.AddAppData(null));
        }


        [Fact]
        public void GetAppData_for_invalid_Id()
        {
            // arrange

            var sizeRepo = NewResponseSizeRepo(Common.GetMockAppRepo());

            // act
            var data = sizeRepo.GetAppData(-1);

            // assert
            Assert.Null(data);
        }

        [Fact]
        public void GetAppData_for_valid()
        {
            // arrange
            var sizeRepo = NewResponseSizeRepo(Common.GetMockAppRepo());
            var data = new AppResponseSize()
            {
                AppId = DefaultAppId,
                ModifiedDate = DateTime.UtcNow,
                Recent = 10,
                Min = 5,
                Max = 20,
                Total = 6,
                Scale = Size.KB
            };
            data = sizeRepo.AddAppData(data);

            // act
            var actual = sizeRepo.GetAppData(data.Id);

            // assert
            Assert.NotNull(actual);
            Assert.NotSame(actual, data);
            Assert.Equal(actual.Id, data.Id);
            Assert.True(actual.Id > 0);
            Assert.True(actual.AppId == data.AppId);
            Assert.True(actual.Recent == data.Recent);
        }

        [Fact]
        public void GetAppDataById_for_valid()
        {
            // arrange
            var sizeRepo = NewResponseSizeRepo(Common.GetMockAppRepo());
            var data = new AppResponseSize()
            {
                AppId = DefaultAppId,
                ModifiedDate = DateTime.UtcNow,
                Recent = 10,
                Min = 5,
                Max = 20,
                Total = 6,
                Scale = Size.MB
            };
            data = sizeRepo.AddAppData(data);

            // act
            var actual = sizeRepo.GetAppDataById(data.AppId);

            // assert
            Assert.NotNull(actual);
            Assert.NotSame(actual, data);
            Assert.Equal(actual.Id, data.Id);
            Assert.True(actual.Id > 0);
            Assert.True(actual.AppId == data.AppId);
            Assert.True(actual.Recent == data.Recent);
            Assert.True(actual.Scale == data.Scale);
        }

        [Fact]
        public void GetAppDataById_for_invalid_Id()
        {
            // arrange

            var sizeRepo = NewResponseSizeRepo(Common.GetMockAppRepo());

            // act
            var data = sizeRepo.GetAppDataById(0);

            // assert
            Assert.Null(data);
        }

        [Fact]
        public void UpdateRecentByAppId_for_existing_app()
        {
            // arrange
            var sizeRepo = NewResponseSizeRepo(Common.GetMockAppRepo());
            var data = new AppResponseSize()
            {
                AppId = DefaultAppId,
                ModifiedDate = DateTime.UtcNow,
                Recent = 10,
                Min = 10,
                Max = 10,
                Avg = 10,
                Total = 1,
                Scale = Size.GB
            };
            data = sizeRepo.AddAppData(data);

            // act
            var actual = sizeRepo.UpdateRecentByAppId(data.AppId, 0);

            // assert
            Assert.NotNull(actual);
            Assert.NotSame(actual, data);
            Assert.Equal(actual.Id, data.Id);
            Assert.True(actual.Id > 0);
            Assert.True(actual.AppId == data.AppId);
            Assert.True(actual.Recent == 0);
        }

        [Fact]
        public void UpdateRecentByAppId_for_new_app()
        {
            // arrange
            var sizeRepo = NewResponseSizeRepo(Common.GetMockAppRepo());

            // act
            var actual = sizeRepo.UpdateRecentByAppId(DefaultAppId, 45.3465);

            // assert
            Assert.NotNull(actual);
            Assert.True(actual.Id > 0);
            Assert.True(actual.AppId == DefaultAppId);
            Assert.True(actual.Recent == 45.3465);
        }

        [Fact]
        public void UpdateRecentByAppId_for_invalid_app()
        {
            // arrange
            var sizeRepo = NewResponseSizeRepo(Common.GetMockAppRepo());

            // act and assert
            Assert.Throws<ArgumentException>(() => sizeRepo.UpdateRecentByAppId(-1, 324.34));

        }
    }
}
