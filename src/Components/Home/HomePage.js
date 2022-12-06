import background from "../../Image/background/background.jpg";

const HomePage = () => {
  return (
    <>
      <div
        className="content-wrapper iframe-mode"
        data-widget="iframe"
        style={{ height: "395px" }}
      >
        <div className="tab-content">
          <div
            className="tab-empty"
            style={{
              backgroundImage: `url(${background})`,
              backgroundRepeat: "no-repeat",
              backgroundSize: "cover",
              height: "93.84vh",
            }}
          >
            <div
              style={{
                top: "20px",
                position: "absolute",
                fontSize: "50px",
                fontWeight: "bold",
                color: "rgb(235 125 76 / 70%)",
              }}
            >
              TBS LOGISTICS
            </div>
            <div
              style={{
                position: "absolute",
                top: "80px",
                fontSize: "45PX",
                fontWeight: "bold",
                color: "rgb(235 125 76 / 70%)",
              }}
            >
              TRANSPORT MANAGEMENT SYSTEM
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default HomePage;
