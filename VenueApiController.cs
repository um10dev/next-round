/* SOME CODE HAS BEEN EXTRACTED FOR UPLOAD TO GITHUB */

    [ApiController]
    public class VenueApiController : BaseApiController
    {
        private IAuthenticationService<int> _authenticationService;
        private IVenueService _venuesService;

        public VenueApiController(IVenueService venuesService, IAuthenticationService<int> authenticationService, ILogger<VenueApiController> logger) : base(logger)
        {
            _venuesService = venuesService;
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Insert(VenueAddRequest model)
        {
            try
            {
                ItemResponse<int> resp = new ItemResponse<int>();
                resp.Item = _venuesService.Insert(model, _authenticationService.GetCurrentUserId());
                return Created201(resp);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(int id, VenueUpdateRequest model)
        {
            try
            {
                int userId = _authenticationService.GetCurrentUserId();
                if (id == model.Id)
                {
                    SuccessResponse resp = new SuccessResponse();
                    _venuesService.Update(model, userId);
                    return Ok200(resp);
                }
                else
                {
                    return NotFound404(new ErrorResponse("Bad Request: Body Id does not match Entity"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }

        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<Venue>>> GetAll(string query, int type, int pageIndex, int pageSize)
        {
            try
            {
                Paged<Venue> pagedData = _venuesService.GetAll(query, type, pageIndex, pageSize);

                if (pagedData == null)
                {
                    return StatusCode(404, new ErrorResponse("Record not found."));
                }
                else
                {
                    ItemResponse<Paged<Venue>> resp = new ItemResponse<Paged<Venue>>();
                    resp.Item = pagedData;
                    return Ok200(resp);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }
    }
