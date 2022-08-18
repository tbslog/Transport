import { useState, useEffect } from "react";
import axios from "axios";

const CreateCustommer = () => {
  const [IsLoading, SetIsLoading] = useState(true);
  const [CusId, SetCusId] = useState("");
  const [CusName, SetCusName] = useState("");
  const [MST, SetMST] = useState("");
  const [SDT, SetSDT] = useState("");
  const [GPS, SetGPS] = useState("");
  const [Address, SetAddress] = useState("");
  const [Province, SetProvince] = useState("");
  const [District, SetDistrict] = useState("");
  const [Ward, SetWard] = useState("");

  const [ListProvince, SetListProvince] = useState([]);
  const [ListDistrict, SetListDistrict] = useState([]);
  const [ListWard, SetListWard] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    SetListProvince([]);
    SetListDistrict([]);
    SetListWard([]);
    async function getlistProvince() {
      const listProvince = await axios.get(
        "http://localhost:8088/api/address/ListProvinces"
      );

      if (listProvince && listProvince.data && listProvince.data.length > 0) {
        SetListProvince(listProvince.data);
      }
    }

    getlistProvince();
    SetIsLoading(false);
  }, []);

  const HandleChangeProvince = (val) => {
    try {
      SetIsLoading(true);

      if (val === undefined || val === "") {
        SetListDistrict([]);
        SetListWard([]);
        return;
      }
      SetProvince(val);
      async function getListDistrict() {
        const listDistrict = await axios.get(
          `http://localhost:8088/api/address/ListDistricts?ProvinceId=${val}`
        );
        if (listDistrict && listDistrict.data && listDistrict.data.length > 0) {
          SetListDistrict(listDistrict.data);
        } else {
          SetListDistrict([]);
        }
      }
      getListDistrict();
      SetIsLoading(false);
    } catch (error) {}
  };

  const HandleOnchangeDistrict = (val) => {
    try {
      SetIsLoading(true);

      if (val === undefined || val === "") {
        SetListWard([]);
        return;
      }
      SetDistrict(val);
      async function GetListWard() {
        const listWard = await axios.get(
          `http://localhost:8088/api/address/ListWards?DistrictId=${val}`
        );

        if (listWard && listWard.data && listWard.data.length > 0) {
          SetListWard(listWard.data);
        } else {
          SetListWard([]);
        }
      }
      GetListWard();
      SetIsLoading(false);
    } catch (error) {}
  };

  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Thêm Mới Khách Hàng</h3>
          <div>{IsLoading === true && <div>Loading...</div>}</div>
        </div>

        <form>
          <div className="card-body">
            <div className="form-group">
              <label htmlFor="MaKH">Mã khách hàng</label>
              <input
                type="text"
                className="form-control"
                id="MaKH"
                placeholder="Nhập mã khách hàng"
                onChange={(e) => SetCusId(e.target.value)}
              />
            </div>
            <div className="form-group">
              <label htmlFor="TenKH">Tên khách hàng</label>
              <input
                type="text"
                className="form-control"
                id="TenKH"
                placeholder="Nhập tên khách hàng"
                onChange={(e) => SetCusName(e.target.value)}
              />
            </div>
            <div className="form-group">
              <label htmlFor="MST">Mã số thuế</label>
              <input
                type="text "
                className="form-control"
                id="MST"
                placeholder="Nhập mã số thuế"
                onChange={(e) => SetMST(e.target.value)}
              />
            </div>
            <div className="form-group">
              <label htmlFor="SDT">Số điện thoại</label>
              <input
                type="text"
                className="form-control"
                id="SDT"
                placeholder="Nhập số điện thoại"
                onChange={(e) => SetSDT(e.target.value)}
              />
            </div>
            <div className="form-group">
              <label htmlFor="GPS">Mã GPS</label>
              <input
                type="text"
                className="form-control"
                id="GPS"
                placeholder="Nhập mã GPS"
                onChange={(e) => SetGPS(e.target.value)}
              />
            </div>
            <div className="row">
              <div className="col-sm">
                <div className="form-group">
                  <label htmlFor="Sonha">Số nhà</label>
                  <input
                    type="text"
                    className="form-control"
                    id="Sonha"
                    placeholder="Nhập số nhà"
                    onChange={(e) => SetAddress(e.target.value)}
                  />
                </div>
              </div>
              <div className="col-sm">
                <div className="form-group">
                  <label htmlFor="Tinh">Tỉnh</label>
                  <select
                    className="form-control"
                    id="inputGroupSelect01"
                    onChange={(e) => HandleChangeProvince(e.target.value)}
                  >
                    <option selected value="">
                      Chọn tỉnh...
                    </option>
                    {ListProvince &&
                      ListProvince.length > 0 &&
                      ListProvince.map((val) => {
                        return (
                          <option key={val.maTinh} value={val.maTinh}>
                            {val.tenTinh}
                          </option>
                        );
                      })}
                  </select>
                </div>
              </div>
              <div className="col-sm">
                <div className="form-group">
                  <label htmlFor="SDT">Huyện</label>
                  <select
                    className="form-control"
                    id="inputGroupSelect01"
                    onChange={(e) => HandleOnchangeDistrict(e.target.value)}
                  >
                    {ListDistrict &&
                      ListDistrict.length > 0 &&
                      ListDistrict.map((val) => {
                        return (
                          <option key={val.maHuyen} value={val.maHuyen}>
                            {val.tenHuyen}
                          </option>
                        );
                      })}
                  </select>
                </div>
              </div>
              <div className="col-sm">
                <div className="form-group">
                  <label htmlFor="SDT">Phường</label>
                  <select
                    className="form-control"
                    id="inputGroupSelect01"
                    onChange={(e) => SetWard(e.target.value)}
                  >
                    {ListWard &&
                      ListWard.length > 0 &&
                      ListWard.map((val) => {
                        return (
                          <option key={val.maPhuong} value={val.maPhuong}>
                            {val.tenPhuong}
                          </option>
                        );
                      })}
                  </select>
                </div>
              </div>
            </div>
          </div>
          <div className="card-footer">
            <button type="submit" className="btn btn-primary">
              Submit
            </button>
          </div>
        </form>
      </div>
    </>
  );
};

export default CreateCustommer;
