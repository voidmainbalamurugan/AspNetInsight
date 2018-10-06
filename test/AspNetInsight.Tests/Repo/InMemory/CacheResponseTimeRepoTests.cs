using AspNetInsight.Dto;
using AspNetInsight.Repo;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace AspNetInsight.Tests.Repo.InMemory
{
    /// <summary>
    /// CacheResponseTime unit test cases
    /// </summary>
    public class CacheResponseTimeRepoTests
    {
        public virtual IResponseRepo<AppResponseTime> NewResponseTimeRepo(IAppRepo repo = null)
        {
            DefaultAppId = 123;
            return new CacheResponseTimeRepo(repo);
        }
        public long DefaultAppId { get; protected set; }
        
        [Fact]
        public void AddAppData_for_valid()
        {
            // arrange
            var responseRepo = NewResponseTimeRepo(Common.GetMockAppRepo());
            var data = new AppResponseTime()
            { AppId = DefaultAppId, ModifiedDate = DateTime.UtcNow, Recent = 10,
                Min = 5, Max= 20, Total = 6, Slice = TimeSlice.Milliseconds};

            // act
            var actual = responseRepo.AddAppData(data);

            // assert
            Assert.NotNull(actual);
            Assert.NotSame(actual, data);
            Assert.NotEqual(actual.Id, data.Id);
            Assert.True(actual.Id > 0);
            Assert.True(actual.AppId == data.AppId);
            Assert.True(actual.Recent == data.Recent);
            Assert.True(actual.Slice == data.Slice);
        }

        [Fact]
        public void AddAppData_with_invalid_input()
        {
            // arrange

            var responseRepo = NewResponseTimeRepo(Common.GetMockAppRepo());

            // act and assert
            Assert.Throws<ArgumentNullException>(() => responseRepo.AddAppData(null));
        }


        [Fact]
        public void GetAppData_for_invalid_Id()
        {
            // arrange
            
            var responseRepo = NewResponseTimeRepo(Common.GetMockAppRepo());

            // act
            var data  = responseRepo.GetAppData(-1);

            // assert
            Assert.Null(data);
        }

        [Fact]
        public void GetAppData_for_valid()
        {
            // arrange
            var responseRepo = NewResponseTimeRepo(Common.GetMockAppRepo());
            var data = new AppResponseTime()
            {
                AppId = DefaultAppId,
                ModifiedDate = DateTime.UtcNow,
                Recent = 10,
                Min = 5,
                Max = 20,
                Total = 6,
                Slice = TimeSlice.Milliseconds
            };
            data = responseRepo.AddAppData(data);

            // act
            var actual = responseRepo.GetAppData(data.Id);

            // assert
            Assert.NotNull(actual);
            Assert.NotSame(actual, data);
            Assert.Equal(actual.Id, data.Id);
            Assert.True(actual.Id > 0);
            Assert.True(actual.AppId == data.AppId);
            Assert.True(actual.Recent == data.Recent);
            Assert.True(actual.Slice == data.Slice);
        }

        [Fact]
        public void GetAppDataById_for_valid()
        {
            // arrange
            var responseRepo = NewResponseTimeRepo(Common.GetMockAppRepo());
            var data = new AppResponseTime()
            {
                AppId = DefaultAppId,
                ModifiedDate = DateTime.UtcNow,
                Recent = 10,
                Min = 5,
                Max = 20,
                Total = 6,
                Slice = TimeSlice.Milliseconds
            };
            data = responseRepo.AddAppData(data);

            // act
            var actual = responseRepo.GetAppDataById(data.AppId);

            // assert
            Assert.NotNull(actual);
            Assert.NotSame(actual, data);
            Assert.Equal(actual.Id, data.Id);
            Assert.True(actual.Id > 0);
            Assert.True(actual.AppId == data.AppId);
            Assert.True(actual.Recent == data.Recent);
            Assert.True(actual.Slice == data.Slice);
        }

        [Fact]
        public void GetAppDataById_for_invalid_Id()
        {
            // arrange

            var responseRepo = NewResponseTimeRepo(Common.GetMockAppRepo());

            // act
            var data = responseRepo.GetAppDataById(0);

            // assert
            Assert.Null(data);
        }

        [Fact]
        public void UpdateRecentByAppId_for_existing_app()
        {
            // arrange
            var responseRepo = NewResponseTimeRepo(Common.GetMockAppRepo());
            var data = new AppResponseTime()
            {
                AppId = DefaultAppId,
                ModifiedDate = DateTime.UtcNow,
                Recent = 10,
                Min = 10,
                Max = 10,
                Avg = 10,
                Total = 1,
                Slice = TimeSlice.Milliseconds
            };
            data = responseRepo.AddAppData(data);
            
            // act
            var actual = responseRepo.UpdateRecentByAppId(data.AppId, 0);

            // assert
            Assert.NotNull(actual);
            Assert.NotSame(actual, data);
            Assert.Equal(actual.Id, data.Id);
            Assert.True(actual.Id > 0);
            Assert.True(actual.AppId == data.AppId);
            Assert.True(actual.Recent == 0);
            Assert.True(actual.Slice == data.Slice);
        }

        [Fact]
        public void UpdateRecentByAppId_for_new_app()
        {
            // arrange
            var responseRepo = NewResponseTimeRepo(Common.GetMockAppRepo());
            
            // act
            var actual = responseRepo.UpdateRecentByAppId(DefaultAppId, 45.3465);

            // assert
            Assert.NotNull(actual);
            Assert.True(actual.Id > 0);
            Assert.True(actual.AppId == DefaultAppId);
            Assert.True(actual.Recent == 45.3465);
            Assert.True(actual.Slice == TimeSlice.Milliseconds);
        }

        [Fact]
        public void UpdateRecentByAppId_for_invalid_app()
        {
            // arrange
            var responseRepo = NewResponseTimeRepo(Common.GetMockAppRepo());

            // act and assert
            Assert.Throws<ArgumentException>(() => responseRepo.UpdateRecentByAppId(-1, 324.34));            
            
        }
    }
}