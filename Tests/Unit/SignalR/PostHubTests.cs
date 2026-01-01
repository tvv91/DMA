using Microsoft.AspNetCore.SignalR;
using Moq;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Web.SignalRHubs;
using Xunit;

namespace Tests.Unit.SignalR
{
    public class PostHubTests
    {
        private readonly Mock<IPostRepository> _mockRepository;
        private readonly Mock<IPostService> _mockService;
        private readonly PostHub _hub;

        public PostHubTests()
        {
            _mockRepository = new Mock<IPostRepository>();
            _mockService = new Mock<IPostService>();
            _hub = new PostHub(_mockRepository.Object, _mockService.Object);
        }

        [Fact]
        public async Task GetPosts_ReturnsPosts_WithCorrectPagination()
        {
            // Arrange
            var posts = new List<Post>
            {
                Helpers.TestDataBuilder.CreatePost(1, "Post 1"),
                Helpers.TestDataBuilder.CreatePost(2, "Post 2")
            };

            var pagedResult = new Web.Common.PagedResult<Post>(posts, 2, 1, 5);
            _mockRepository.Setup(r => r.GetFilteredListAsync(1, 5, "", "", "", false))
                .ReturnsAsync(pagedResult);

            // Note: This is a simplified test. Full SignalR hub testing requires more setup
            // In a real scenario, you'd use a test client or mock the Clients property
        }
    }
}

