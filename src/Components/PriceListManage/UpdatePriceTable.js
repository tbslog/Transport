import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import Select from "react-select";

const UpdatePriceTable = (props) => {
  const { selectIdClick, hideModal, refeshData } = props;

  const {
    register,
    setValue,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    MaHopDong: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    MaKh: {
      required: "Không được để trống",
      maxLength: {
        value: 8,
        message: "Không được vượt quá 8 ký tự",
      },
      minLength: {
        value: 8,
        message: "Không được ít hơn 8 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    MaCungDuong: {
      required: "Không được để trống",
    },
    NgayHetHieuLuc: {
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 10 ký tự",
      },
      pattern: {
        value:
          /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/,
        message: "Không phải định dạng ngày",
      },
    },
    DonGia: {
      pattern: {
        value: /^[0-9]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
      required: "Không được để trống",
    },
    MaLoaiPhuongTien: {
      required: "Không được để trống",
    },
    MaLoaiHangHoa: {
      required: "Không được để trống",
    },
    MaDVT: {
      required: "Không được để trống",
    },
    MaPTVC: {
      required: "Không được để trống",
    },
    TrangThai: {
      required: "Không được để trống",
    },
    PhanLoaiDoiTac: {
      required: "Không được để trống",
    },
  };

  const [IsLoading, SetIsLoading] = useState(false);
  const [listRoad, setListRoad] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listVehicleType, setListVehicleType] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listDVT, setListDVT] = useState([]);
  const [listTransportType, setListTransportType] = useState([]);
  const [listContract, setListContract] = useState([]);
  const [listCustomerType, setListCustomerType] = useState([]);
  const [listArea, setListArea] = useState([]);

  useEffect(() => {
    SetIsLoading(true);

    (async () => {
      let getListDVT = await getData("Common/GetListDVT");
      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");
      let getListTransportType = await getData("Common/GetListTransportType");
      let getListCustommerType = await getData(`Common/GetListCustommerType`);
      const getListArea = await getData("Common/GetListArea");
      setListArea(getListArea);
      setListCustomerType(getListCustommerType);
      setListVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);
      setListDVT(getListDVT);
      setListTransportType(getListTransportType);
      SetIsLoading(false);
    })();
  }, []);

  useEffect(() => {
    if (
      selectIdClick &&
      Object.keys(selectIdClick).length > 0 &&
      listVehicleType &&
      listGoodsType &&
      listDVT &&
      listTransportType &&
      listCustomerType &&
      listVehicleType.length > 0 &&
      listGoodsType.length > 0 &&
      listDVT.length > 0 &&
      listTransportType.length > 0 &&
      listCustomerType.length > 0
    ) {
      SetIsLoading(true);
      setValue("PhanLoaiDoiTac", selectIdClick.maLoaiDoiTac);
      handleOnChangeContractType(selectIdClick.maLoaiDoiTac);
    }
  }, [
    selectIdClick,
    listVehicleType,
    listGoodsType,
    listDVT,
    listTransportType,
    listCustomerType,
  ]);

  useEffect(() => {
    if (
      listCustomer &&
      selectIdClick &&
      listCustomer.length > 0 &&
      Object.keys(selectIdClick).length > 0
    ) {
      setValue(
        "MaKh",
        { ...listCustomer.filter((x) => x.value === selectIdClick.maKh) }[0]
      );

      getListRoadAndContract(selectIdClick.maKh);
    }
  }, [listCustomer, selectIdClick]);

  useEffect(() => {
    if (
      selectIdClick &&
      listContract &&
      listRoad &&
      listArea &&
      listArea.length > 0 &&
      Object.keys(selectIdClick).length > 0 &&
      listContract.length > 0 &&
      listRoad.length > 0
    ) {
      setValue(
        "MaHopDong",
        {
          ...listContract.filter((x) => x.value === selectIdClick.maHopDong),
        }[0]
      );

      setValue(
        "MaCungDuong",
        {
          ...listRoad.filter((x) => x.value === selectIdClick.maCungDuong),
        }[0]
      );
      setValue("DonGia", selectIdClick.donGia);
      setValue("MaDVT", selectIdClick.maDVT);
      setValue("MaPTVC", selectIdClick.maPTVC);
      setValue("MaLoaiPhuongTien", selectIdClick.maLoaiPhuongTien);
      setValue("MaLoaiHangHoa", selectIdClick.maLoaiHangHoa);
      setValue("NgayHetHieuLuc", selectIdClick.ngayHetHieuLuc);
      setValue("KhuVuc", selectIdClick.maKhuVuc);

      SetIsLoading(false);
    }
  }, [selectIdClick, listContract, listRoad, listArea]);

  const handleOnchangeListCustomer = (val) => {
    SetIsLoading(true);

    setListContract([]);
    setListRoad([]);
    setValue("optionRoad", [{ MaCungDuong: null }]);
    setValue("MaKh", val);
    setValue("MaHopDong", null);
    getListRoadAndContract(val.value);

    SetIsLoading(false);
  };

  const handleOnChangeContractType = async (val) => {
    setValue("PhanLoaiDoiTac", val);
    if (val && val !== "") {
      let getListCustomer = await getData(
        `Customer/GetListCustomerOptionSelect`
      );
      if (getListCustomer && getListCustomer.length > 0) {
        getListCustomer = getListCustomer.filter((x) => x.loaiKH === val);
        let obj = [];

        getListCustomer.map((val) => {
          obj.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh,
          });
        });
        setListCustomer(obj);
      }
    }
  };

  const getListRoadAndContract = async (MaKh) => {
    SetIsLoading(true);
    let getListRoad = await getData(`Road/GetListRoadOptionSelect`);
    if (getListRoad && getListRoad.length > 0) {
      let obj = [];
      getListRoad.map((val) => {
        obj.push({
          value: val.maCungDuong,
          label: val.maCungDuong + " - " + val.tenCungDuong,
        });
      });
      setListRoad(obj);
    }

    let getListContract = await getData(
      `Contract/GetListContractSelect?MaKH=${MaKh}`
    );

    if (getListContract && getListContract.length > 0) {
      let obj = [];

      getListContract.map((val) => {
        obj.push({
          value: val.maHopDong,
          label: val.maHopDong + " - " + val.tenHienThi,
        });
      });
      setListContract(obj);
    }

    SetIsLoading(false);
  };

  const onSubmit = async (data, e) => {
    SetIsLoading(true);

    const updatePriceTable = await postData(
      `PriceTable/UpdatePriceTable?id=${selectIdClick.id}`,
      {
        MaHopDong: data.MaHopDong.value,
        MaPTVC: data.MaPTVC,
        MaCungDuong: data.MaCungDuong.value,
        MaLoaiPhuongTien: data.MaLoaiPhuongTien,
        DonGia: data.DonGia,
        MaKhuVuc: !data.KhuVuc ? null : data.KhuVuc,
        MaDVT: data.MaDVT,
        MaLoaiHangHoa: data.MaLoaiHangHoa,
        NgayHetHieuLuc: !data.NgayHetHieuLuc
          ? null
          : moment(new Date(data.NgayHetHieuLuc).toISOString()).format(
              "YYYY-MM-DD"
            ),
      }
    );
    if (updatePriceTable === 1) {
      refeshData(1);
      hideModal();
    }

    SetIsLoading(false);
  };

  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Thêm Mới Bảng Giá</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="PhanLoaiDoiTac">Phân Loại Đối Tác(*)</label>
                    <select
                      disabled={true}
                      className="form-control"
                      {...register("PhanLoaiDoiTac", Validate.PhanLoaiDoiTac)}
                      onChange={(e) =>
                        handleOnChangeContractType(e.target.value)
                      }
                    >
                      <option value="">Chọn phân loại đối tác</option>
                      {listCustomerType &&
                        listCustomerType.map((val) => {
                          return (
                            <option value={val.maLoaiKh} key={val.maLoaiKh}>
                              {val.tenLoaiKh}
                            </option>
                          );
                        })}
                    </select>
                    {errors.PhanLoaiDoiTac && (
                      <span className="text-danger">
                        {errors.PhanLoaiDoiTac.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="KhachHang">
                      Khách Hàng / Nhà Cung Cấp(*)
                    </label>
                    <Controller
                      name="MaKh"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          isDisabled={true}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listCustomer}
                          onChange={(field) =>
                            handleOnchangeListCustomer(field)
                          }
                        />
                      )}
                      rules={Validate.MaKh}
                    />
                    {errors.MaKh && (
                      <span className="text-danger">{errors.MaKh.message}</span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaHopDong">Hợp Đồng(*)</label>
                    <Controller
                      name="MaHopDong"
                      rules={Validate.MaHopDong}
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listContract}
                        />
                      )}
                    />
                    {errors.MaHopDong && (
                      <span className="text-danger">
                        {errors.MaHopDong.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <br />
              <table
                className="table table-sm table-bordered"
                style={{
                  whiteSpace: "nowrap",
                }}
              >
                <thead>
                  <tr>
                    <th>Cung Đường(*)</th>
                    <th>Khu Vực</th>
                    <th>Đơn Giá(*)</th>
                    <th>Đơn vị tính(*)</th>
                    <th>Phương Thức Vận Chuyển(*)</th>
                    <th>Loại phương tiện(*)</th>
                    <th>Loại Hàng Hóa(*)</th>
                    <th>Ngày Hết Hiệu Lực</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>
                      <div className="form-group">
                        <Controller
                          name={`MaCungDuong`}
                          control={control}
                          render={({ field }) => (
                            <Select
                              {...field}
                              classNamePrefix={"form-control"}
                              value={field.value}
                              options={listRoad}
                              defaultValue={null}
                            />
                          )}
                          rules={{ required: "không được để trống" }}
                        />
                        {errors.MaCungDuong && (
                          <span className="text-danger">
                            {errors.MaCungDuong.message}
                          </span>
                        )}
                      </div>
                    </td>
                    <td>
                      <div>
                        <div className="col-sm">
                          <div className="form-group">
                            <select
                              className="form-control"
                              {...register("KhuVuc", Validate.KhuVuc)}
                            >
                              <option value="">-- Bỏ Trống --</option>
                              {listArea &&
                                listArea.length > 0 &&
                                listArea.map((val, index) => {
                                  return (
                                    <option value={val.id} key={index}>
                                      {val.tenKhuVuc}
                                    </option>
                                  );
                                })}
                            </select>
                            {errors.KhuVuc && (
                              <span className="text-danger">
                                {errors.KhuVuc.message}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    </td>
                    <td>
                      <div className="form-group">
                        <input
                          type="text"
                          className="form-control"
                          id="DonGia"
                          {...register(`DonGia`, Validate.DonGia)}
                        />
                        {errors.DonGia && (
                          <span className="text-danger">
                            {errors.DonGia.message}
                          </span>
                        )}
                      </div>
                    </td>
                    <td>
                      <div className="form-group">
                        <select
                          className="form-control"
                          {...register(`MaDVT`, Validate.MaDVT)}
                        >
                          <option value="">Chọn đơn vị tính</option>
                          {listDVT &&
                            listDVT.map((val) => {
                              return (
                                <option value={val.maDvt} key={val.maDvt}>
                                  {val.tenDvt}
                                </option>
                              );
                            })}
                        </select>
                        {errors.MaDVT && (
                          <span className="text-danger">
                            {errors.MaDVT.message}
                          </span>
                        )}
                      </div>
                    </td>
                    <td>
                      <div className="form-group">
                        <select
                          className="form-control"
                          {...register(`MaPTVC`, Validate.MaPTVC)}
                        >
                          <option value="">Chọn phương thức vận chuyển</option>
                          {listTransportType &&
                            listTransportType.map((val) => {
                              return (
                                <option value={val.maPtvc} key={val.maPtvc}>
                                  {val.tenPtvc}
                                </option>
                              );
                            })}
                        </select>
                        {errors.MaPTVC && (
                          <span className="text-danger">
                            {errors.MaPTVC.message}
                          </span>
                        )}
                      </div>
                    </td>
                    <td>
                      <div className="form-group">
                        <select
                          className="form-control"
                          {...register(
                            `MaLoaiPhuongTien`,
                            Validate.MaLoaiPhuongTien
                          )}
                        >
                          <option value="">Chọn loại phương tiện</option>
                          {listVehicleType &&
                            listVehicleType.map((val) => {
                              return (
                                <option
                                  value={val.maLoaiPhuongTien}
                                  key={val.maLoaiPhuongTien}
                                >
                                  {val.tenLoaiPhuongTien}
                                </option>
                              );
                            })}
                        </select>
                        {errors.MaLoaiPhuongTien && (
                          <span className="text-danger">
                            {errors.MaLoaiPhuongTien.message}
                          </span>
                        )}
                      </div>
                    </td>
                    <td>
                      <div className="form-group">
                        <select
                          className="form-control"
                          {...register(`MaLoaiHangHoa`, Validate.MaLoaiHangHoa)}
                        >
                          <option value="">Chọn loại hàng hóa</option>
                          {listGoodsType &&
                            listGoodsType.map((val) => {
                              return (
                                <option
                                  value={val.maLoaiHangHoa}
                                  key={val.maLoaiHangHoa}
                                >
                                  {val.tenLoaiHangHoa}
                                </option>
                              );
                            })}
                        </select>
                        {errors.MaLoaiHangHoa && (
                          <span className="text-danger">
                            {errors.MaLoaiHangHoa.message}
                          </span>
                        )}
                      </div>
                    </td>

                    <td>
                      <div className="form-group">
                        <div className="input-group ">
                          <Controller
                            control={control}
                            name={`NgayHetHieuLuc`}
                            render={({ field }) => (
                              <DatePicker
                                className="form-control"
                                dateFormat="dd/MM/yyyy"
                                onChange={(date) => field.onChange(date)}
                                selected={field.value}
                              />
                            )}
                            rules={Validate.NgayHetHieuLuc}
                          />
                          {errors.NgayHetHieuLuc && (
                            <span className="text-danger">
                              {errors.NgayHetHieuLuc.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
              <br />
            </div>
            <div className="card-footer">
              <div>
                <button
                  type="submit"
                  className="btn btn-primary"
                  style={{ float: "right" }}
                >
                  Cập Nhật
                </button>
              </div>
            </div>
          </form>
        )}
      </div>
    </>
  );
};

export default UpdatePriceTable;
