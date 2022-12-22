const HeaderPage = () => {
  return (
    <div className="content-header">
      <div className="container" style={{ maxWidth: "100%" }}>
        <div className="row mb-2">
          <div className="col-sm-6">
            <h4 className="m-0">Home Page</h4>
          </div>
          <div className="col-sm-6">
            <ol className="breadcrumb float-sm-right">
              <li className="breadcrumb-item">
                <a href="#">Home</a>
              </li>
              <li className="breadcrumb-item">
                <a href="#">Layout</a>
              </li>
              <li className="breadcrumb-item active">Top Navigation</li>
            </ol>
          </div>
        </div>
      </div>
    </div>
  );
};
export default HeaderPage;
