class Venues extends Component {
  state = {
    pageIndex: 0,
    pageSize: 8,
    venuesArray: [],
    mappedVenues: [],
    modifiedList: [],
    venueCity: "",
    venueState: "",
    totalPages: 0,
    search: {
      query: "",
      type: 0
    },
    searching: false
  };

  componentDidMount() {
    this.getAllVenues();
  }

  getAllVenues = () => {
    venuesService
      .getAllVenues(this.state.pageIndex, this.state.pageSize)
      .then(this.onSuccess)
      .catch(this.onError);
  };

  onSuccess = response => {
    this.setState(() => {
      return {
        totalPages: response.item.totalPages,
        venuesArray: response.item.pagedItems,
        mappedVenues: response.item.pagedItems.map(this.mapVenue)
      };
    });
  };

  mapVenue = (venue, index) => (
    <VenueProfile
      index={index}
      venue={venue}
      key={venue.id}
      editVenue={this.editVenue}
      seeMoreVenue={this.seeMoreVenue}
      changeVenueStatus={this.changeVenueStatus}
    />
  );

  onError = err => {
    _logger(err);
  };

  onCreateClick = () => {
    this.props.history.push("/venues/create");
  };

  editVenue = venue => {
    this.props.history.push(`/venues/${venue.id}/edit`, { venue });
  };

  seeMoreVenue = venue => {
    this.props.history.push(`/venues/${venue.id}/details`, {
      venue
    });
  };

  changeVenueStatus = (venue, index) => {
    this.props.history.push({ venue });

    if (venue.statusId === 0) {
      venuesService
        .editVenueStatus(1, venue.id)
        .then(this.onStatusSuccess(index))
        .catch(this.onStatusError);
    } else {
      venuesService
        .editVenueStatus(0, venue.id)
        .then(this.onStatusSuccess(index))
        .catch(this.onStatusError);
    }
  };

  onStatusSuccess = index => {
    if (this.state.mappedVenues[index].props.venue.statusId === 0) {
      let newList = [...this.state.mappedVenues];
      newList[index].props.venue.statusId = 1;
      this.setState(() => {
        return {
          modifiedList: newList.map(this.mapVenue)
        };
      });
    } else {
      let newList = [...this.state.mappedVenues];
      newList[index].props.venue.statusId = 0;
      this.setState(() => {
        return {
          modifiedList: newList.map(this.mapVenue)
        };
      });
    }
  };

  onStatusError = err => {
    _logger(err, "This is the Error.");
  };

/* SOME CODE HAS BEEN EXTRACTED FOR UPLOAD TO GITHUB */

  onSearchClick = () => {
    const query = this.state.search.query;
    const type = this.state.search.type;
    const pageIndex = this.state.pageIndex;
    const pageSize = this.state.pageSize;

    venuesService
      .getSearchedVenues(query, type, pageIndex, pageSize)
      .then(this.onSearchVenuesSuccess)
      .catch(this.onSearchVenuesError);
  };

  onSearchVenuesSuccess = response => {
    this.setState(prevState => {
      return {
        totalPages: response.item.totalPages,
        venuesArray: response.item.pagedItems,
        mappedVenues: response.item.pagedItems.map(this.mapVenue),
        search: { ...prevState.search, query: "", type: 0 },
        searching: true
      };
    });
  };

  onSearchVenuesError = err => {
    _logger(err);
  };

  onAllVenuesClick = () => {
    this.setState(() => {
      return { searching: false };
    });
    this.getAllVenues();
  };

/* SOME CODE HAS BEEN EXTRACTED FOR UPLOAD TO GITHUB */
