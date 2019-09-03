{
    public class VenueService : IVenueService
    {
        private IDataProvider _dataProvider;

        public VenueService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public int Insert(VenueAddRequest model, int userId)
        {
            int retVal = 0;

            _dataProvider.ExecuteNonQuery("dbo.Venues_Insert", inputParamMapper: delegate (SqlParameterCollection parms)
            {
                SqlParameter parm = new SqlParameter();
                parm.ParameterName = "@Id";
                parm.SqlDbType = SqlDbType.Int;
                parm.Direction = ParameterDirection.Output;
                parms.Add(parm);

                parms.AddWithValue("@Name", model.Name);
                parms.AddWithValue("@Description", model.Description);
                parms.AddWithValue("@LocationId", model.LocationId);
                parms.AddWithValue("@Url", model.Url);
                parms.AddWithValue("@AvatarUrl", model.AvatarUrl);
                parms.AddWithValue("@CreatedBy", userId);  
                parms.AddWithValue("@ModifiedBy", userId);
            }, returnParameters: delegate (SqlParameterCollection parms)
            {
                Int32.TryParse(parms["@Id"].Value.ToString(), out retVal);
            });

            return retVal;
        }

        public void Update(VenueUpdateRequest model, int userId)
        {
            _dataProvider.ExecuteNonQuery("dbo.Venues_Update", inputParamMapper: delegate (SqlParameterCollection parms)
            {
                parms.AddWithValue("@Id", model.Id);
                parms.AddWithValue("@Name", model.Name);
                parms.AddWithValue("@Description", model.Description);
                parms.AddWithValue("@LocationId", model.LocationId);
                parms.AddWithValue("@Url", model.Url);
                parms.AddWithValue("@AvatarUrl", model.AvatarUrl);
                parms.AddWithValue("@ModifiedBy", userId);
            });
        }}

        public Paged<Venue> GetAll(string query, int type, int pageIndex, int pageSize)
        {
            List<Venue> venuesList = null;
            Paged<Venue> pagedVenues = null;
            int totalCount = 0;

            _dataProvider.ExecuteCmd("dbo.Venues_Select_BySearch_Paginated", inputParamMapper: delegate (SqlParameterCollection parms)
            {
                parms.AddWithValue("@Query", query);
                parms.AddWithValue("@Type", type);
                parms.AddWithValue("@PageIndex", pageIndex);
                parms.AddWithValue("@PageSize", pageSize);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                Venue model = Mapper(reader);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(18);
                }

                if (venuesList == null)
                {
                    venuesList = new List<Venue>();
                }

                venuesList.Add(model);
            });

            if (venuesList != null)
            {
                pagedVenues = new Paged<Venue>(venuesList, pageIndex, pageSize, totalCount);
            }

            return pagedVenues;
        }

        public void Delete(int id)
        {
            _dataProvider.ExecuteNonQuery("dbo.Venues_Delete_ById", inputParamMapper: delegate (SqlParameterCollection parms)
            {
                parms.AddWithValue("@Id", id);
            });
        }
    }
}
