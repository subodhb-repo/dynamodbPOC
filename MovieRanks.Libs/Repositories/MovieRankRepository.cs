using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using MovieRank.Contracts;
using MovieRanks.Libs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieRanks.Libs.Repositories
{
    public class MovieRankRepository
    {
        private readonly DynamoDBContext _context;

        public MovieRankRepository(IAmazonDynamoDB dynamoDBClient)
        {
            _context = new DynamoDBContext(dynamoDBClient);
        }

        /// <summary>
        /// Scan
        /// </summary>
        public async Task<IEnumerable<MovieDb>> GetAllItems()
        {
            return await _context.ScanAsync<MovieDb>(new List<ScanCondition>()).GetRemainingAsync();
        }

        /// <summary>
        /// Load
        /// </summary>
        public async Task<MovieDb> GetMovie(int userID, string movieName)
        {
            return await _context.LoadAsync<MovieDb>(userID, movieName);
        }

        /// <summary>
        /// Query filter
        /// </summary>
        public async Task<IEnumerable<MovieDb>> GetUsersRankedMoviesByMovieTitle(int userID, string movieName)
        {
            var config = new DynamoDBOperationConfig
            {
                QueryFilter = new List<ScanCondition>
                {
                    new ScanCondition("MovieName", ScanOperator.BeginsWith, movieName)
                }
            };

            return await _context.QueryAsync<MovieDb>(userID, config).GetRemainingAsync();
        }

        /// <summary>
        /// Add new entity
        /// </summary>
        public async Task AddMovie(MovieDb movie)
        {
            await _context.SaveAsync<MovieDb>(movie);
        }

        /// <summary>
        /// Add new entity
        /// </summary>
        public async Task UpdateMovie(int userID, MovieUpdateRequest newDetails)
        {
            var movie = await GetMovie(userID, newDetails.MovieName);
            movie.Ranking = newDetails.Ranking;
            await _context.SaveAsync<MovieDb>(movie);
        }

        /// <summary>
        /// Using Secondary index
        /// </summary>
        public async Task<IEnumerable<MovieDb>> GetMovieRank(string movieName)
        {
            var config = new DynamoDBOperationConfig
            {
                IndexName = "MovieName-index",
                
            };

            return await _context.QueryAsync<MovieDb>(movieName, config).GetRemainingAsync();
        }
    }
}
