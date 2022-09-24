import { useState, useEffect, useMemo } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import DataTable from "react-data-table-component";
import moment from "moment";
import Select from "react-select";

const AddPriceTable = (props) => {
  const { getListPriceTable, selectIdClick, listStatus } = props;
  const {
    register,
    reset,
    setValue,
    watch,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
    defaultValues: {
      optionRoad: [
        {
          MaCungDuong: null,
          DonGia: "",
          MaDVT: "",
          // MaPTVC: "",
          MaLoaiPhuongTien: "",
          MaLoaiHangHoa: "",
          NgayApDung: "",
          NgayHetHieuLuc: "",
          TrangThai: "",
        },
      ],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control, // control props comes from useForm (optional: if you are using FormContext)
    name: "optionRoad", // unique name for your Field Array
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
    NgayApDung: {
      required: "Không được để trống",
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
    NgayHetHieuLuc: {
      required: "Không được để trống",
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
    GiaVND: {
      pattern: {
        value: /^[0-9]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
      required: "Không được để trống",
    },
    GiaUSD: {
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
    // MaPTVC: {
    //   required: "Không được để trống",
    // },
    TrangThai: {
      required: "Không được để trống",
    },
    PhanLoaiDoiTac: {
      required: "Không được để trống",
    },
  };

  const columns = useMemo(() => [
    {
      name: "Mã Khách Hàng",
      selector: (row) => row.maKh,
      sortable: true,
    },
    {
      name: "Mã Hợp Đồng",
      selector: (row) => row.maHopDong,
      sortable: true,
    },
    {
      name: "",
      selector: (row) => row.soHopDongCha,
      sortable: true,
    },
    {
      name: "Mã Cung Đường",
      selector: (row) => row.maCungDuong,
      sortable: true,
    },
    {
      name: "Đơn Giá",
      selector: (row) => row.donGia,
      sortable: true,
    },
    {
      name: "Loại Phương Tiện",
      selector: (row) => row.maLoaiPhuongTien,
      sortable: true,
    },
    {
      name: "Loại Hàng Hóa",
      selector: (row) => row.maLoaiHangHoa,
      sortable: true,
    },
    {
      name: "Đơn Vị Tính",
      selector: (row) => row.maDVT,
      sortable: true,
    },
    // {
    //   name: "Phương Thức Vận Chuyển",
    //   selector: (row) => row.maPTVC,
    //   sortable: true,
    // },
    {
      name: "Ngày Áp Dụng",
      selector: (row) => row.ngayApDung,
      sortable: true,
    },
    {
      name: "Ngày Hết Hiệu Lực",
      selector: (row) => row.ngayHetHieuLuc,
      sortable: true,
    },
  ]);

  const [IsLoading, SetIsLoading] = useState(false);
  const [data, setData] = useState([]);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);

  const [contractId, setContractId] = useState("");
  const [tabIndex, setTabIndex] = useState(0);
  const [listRoad, setListRoad] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listVehicleType, setListVehicleType] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listDVT, setListDVT] = useState([]);
  // const [listTransportType, setListTransportType] = useState([]);
  const [listContract, setListContract] = useState([]);
  const [listCustomerType, setListCustomerType] = useState([]);

  useEffect(() => {
    setTabIndex(0);
    setData([]);
    if (
      props &&
      props.selectIdClick &&
      Object.keys(props.selectIdClick).length > 0 &&
      Object.keys(props).length > 0
    ) {
      setContractId(selectIdClick.maHopDong);
      fetchData(1, selectIdClick.maHopDong);
    } else {
      handleResetClick();
    }
  }, [props, selectIdClick]);

  useEffect(() => {
    SetIsLoading(true);

    (async () => {
      let getListDVT = await getData("Common/GetListDVT");
      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");
      // let getListTransportType = await getData("Common/GetListTransportType");

      let getListCustommerType = await getData(`Common/GetListCustommerType`);
      setListCustomerType(getListCustommerType);

      setListVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);
      setListDVT(getListDVT);
      // setListTransportType(getListTransportType);
      SetIsLoading(false);
    })();
  }, []);

  const handlePerRowsChange = async (newPerPage, page) => {
    SetIsLoading(true);
    const dataCus = await getData(
      `PriceTable/GetListPriceTableByContractId?Id=${contractId}&PageNumber=${page}&PageSize=${newPerPage}`
    );

    formatTable(dataCus.data);
    setPerPage(newPerPage);
    setTotalRows(dataCus.totalRecords);
    SetIsLoading(false);
  };
  const fetchData = async (page, mahd) => {
    SetIsLoading(true);

    const dataCus = await getData(
      `PriceTable/GetListPriceTableByContractId?Id=${mahd}&PageNumber=${page}&PageSize=${perPage}`
    );
    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    SetIsLoading(false);
  };

  function formatTable(data) {
    data.map((val) => {
      val.ngayApDung = moment(val.ngayApDung).format("DD/MM/YYYY");
      val.ngayHetHieuLuc = moment(val.ngayHetHieuLuc).format("DD/MM/YYYY");
    });
    setData(data);
  }

  const handlePageChange = async (page) => {
    await fetchData(page, contractId);
  };

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
    } else {
      handleResetClick();
    }
  };

  const getListRoadAndContract = async (MaKh) => {
    SetIsLoading(true);
    let getListRoad = await getData(
      `Road/GetListRoadOptionSelect?MaKH=${MaKh}`
    );
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
      obj.push({
        value: "",
        label: "Chọn Hợp Đồng",
      });
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

  const handleResetClick = () => {
    reset();
    setValue("MaKh", null);
    setValue("MaHopDong", null);
    setListContract([]);
    setListRoad([]);
    setListCustomer([]);
  };

  const onSubmit = async (data, e) => {
    SetIsLoading(true);

    let arr = [];
    data.optionRoad.map((val) => {
      arr.push({
        maHopDong: data.MaHopDong.value,
        maKh: data.MaKh.value,
        maPtvc: "VT",
        maCungDuong: val.MaCungDuong.value,
        maLoaiPhuongTien: val.MaLoaiPhuongTien,
        maLoaiDoiTac: data.PhanLoaiDoiTac,
        donGia: val.DonGia,
        maDvt: val.MaDVT,
        maLoaiHangHoa: val.MaLoaiHangHoa,
        ngayApDung: moment(new Date(val.NgayApDung).toISOString()).format(
          "YYYY-MM-DD"
        ),
        ngayHetHieuLuc: moment(
          new Date(val.NgayHetHieuLuc).toISOString()
        ).format("YYYY-MM-DD"),
        trangThai: val.TrangThai,
      });
    });

    const createPriceTable = await postData("PriceTable/CreatePriceTable", arr);
    if (createPriceTable === 1) {
      getListPriceTable(1);
      reset();
    }

    SetIsLoading(false);
  };

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex);
  };

  return (
    <>
      <Tabs
        selectedIndex={tabIndex}
        onSelect={(index) => HandleOnChangeTabs(index)}
      >
        <TabList>
          <Tab>Tạo Bảng giá</Tab>
          {props.selectIdClick &&
            Object.keys(props.selectIdClick).length > 0 && (
              <Tab>Xem Bảng giá</Tab>
            )}
        </TabList>

        <TabPanel>
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
                        <label htmlFor="PhanLoaiDoiTac">
                          Phân Loại Đối Tác
                        </label>
                        <select
                          className="form-control"
                          {...register(
                            "PhanLoaiDoiTac",
                            Validate.PhanLoaiDoiTac
                          )}
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
                        <label htmlFor="KhachHang">Khách Hàng</label>
                        <Controller
                          name="MaKh"
                          control={control}
                          render={({ field }) => (
                            <Select
                              {...field}
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
                          <span className="text-danger">
                            {errors.MaKh.message}
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="MaHopDong">Hợp Đồng</label>
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
                        <th style={{ width: "10px" }}></th>
                        <th>Cung Đường</th>
                        <th>Đơn Giá</th>
                        <th>Đơn vị tính</th>
                        {/* <th>PTVC</th> */}
                        <th>Loại phương tiện</th>
                        <th>Loại Hàng Hóa</th>
                        <th>Ngày Áp Dụng</th>
                        <th>Ngày Hết Hiệu Lực</th>
                        <th>Trạng Thái</th>
                        <th style={{ width: "40px" }}>
                          <button
                            className="form-control form-control-sm"
                            type="button"
                            onClick={() => append(watch("optionRoad", 0))}
                          >
                            <i className="fas fa-plus"></i>
                          </button>
                        </th>
                      </tr>
                    </thead>
                    <tbody>
                      {fields.map((item, index) => (
                        <tr key={item.id}>
                          <td>{index + 1}</td>
                          <td>
                            <div className="form-group">
                              <Controller
                                name={`optionRoad.${index}.MaCungDuong`}
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
                              {errors.optionRoad?.[index]?.MaCungDuong && (
                                <span className="text-danger">
                                  {
                                    errors.optionRoad?.[index]?.MaCungDuong
                                      .message
                                  }
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <input
                                type="text"
                                className="form-control"
                                id="DonGia"
                                {...register(
                                  `optionRoad.${index}.DonGia`,
                                  Validate.DonGia
                                )}
                              />
                              {errors.optionRoad?.[index]?.DonGia && (
                                <span className="text-danger">
                                  {errors.optionRoad?.[index]?.DonGia.message}
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <select
                                className="form-control"
                                {...register(
                                  `optionRoad.${index}.MaDVT`,
                                  Validate.MaDVT
                                )}
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
                              {errors.optionRoad?.[index]?.MaDVT && (
                                <span className="text-danger">
                                  {errors.optionRoad?.[index]?.MaDVT.message}
                                </span>
                              )}
                            </div>
                          </td>
                          {/* <td>
                            <div className="form-group">
                              <select
                                className="form-control"
                                {...register(
                                  `optionRoad.${index}.MaPTVC`,
                                  Validate.MaPTVC
                                )}
                              >
                                <option value="">
                                  Chọn phương thức vận chuyển
                                </option>
                                {listTransportType &&
                                  listTransportType.map((val) => {
                                    return (
                                      <option
                                        value={val.maPtvc}
                                        key={val.maPtvc}
                                      >
                                        {val.tenPtvc}
                                      </option>
                                    );
                                  })}
                              </select>
                              {errors.optionRoad?.[index]?.MaPTVC && (
                                <span className="text-danger">
                                  {errors.optionRoad?.[index]?.MaPTVC.message}
                                </span>
                              )}
                            </div>
                          </td> */}
                          <td>
                            <div className="form-group">
                              <select
                                className="form-control"
                                {...register(
                                  `optionRoad.${index}.MaLoaiPhuongTien`,
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
                              {errors.optionRoad?.[index]?.MaLoaiPhuongTien && (
                                <span className="text-danger">
                                  {
                                    errors.optionRoad?.[index]?.MaLoaiPhuongTien
                                      .message
                                  }
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <select
                                className="form-control"
                                {...register(
                                  `optionRoad.${index}.MaLoaiHangHoa`,
                                  Validate.MaLoaiHangHoa
                                )}
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
                              {errors.optionRoad?.[index]?.MaLoaiHangHoa && (
                                <span className="text-danger">
                                  {
                                    errors.optionRoad?.[index]?.MaLoaiHangHoa
                                      .message
                                  }
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name={`optionRoad.${index}.NgayApDung`}
                                  render={({ field }) => (
                                    <DatePicker
                                      className="form-control"
                                      dateFormat="dd/MM/yyyy"
                                      onChange={(date) => field.onChange(date)}
                                      selected={field.value}
                                    />
                                  )}
                                  rules={Validate.NgayApDung}
                                />
                                {errors.optionRoad?.[index]?.NgayApDung && (
                                  <span className="text-danger">
                                    {
                                      errors.optionRoad?.[index]?.NgayApDung
                                        .message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <div className="input-group ">
                                <Controller
                                  control={control}
                                  name={`optionRoad.${index}.NgayHetHieuLuc`}
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
                                {errors.optionRoad?.[index]?.NgayHetHieuLuc && (
                                  <span className="text-danger">
                                    {
                                      errors.optionRoad?.[index]?.NgayHetHieuLuc
                                        .message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <select
                                className="form-control"
                                {...register(
                                  `optionRoad.${index}.TrangThai`,
                                  Validate.TrangThai
                                )}
                              >
                                <option value="">Chọn Trạng Thái</option>
                                {listStatus &&
                                  listStatus.map((val) => {
                                    return (
                                      <option
                                        value={val.statusId}
                                        key={val.statusId}
                                      >
                                        {val.statusContent}
                                      </option>
                                    );
                                  })}
                              </select>
                              {errors.optionRoad?.[index]?.TrangThai && (
                                <span className="text-danger">
                                  {
                                    errors.optionRoad?.[index]?.TrangThai
                                      .message
                                  }
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              {index >= 1 && (
                                <button
                                  type="button"
                                  className="form-control form-control-sm"
                                  onClick={() => remove(index)}
                                >
                                  <i className="fas fa-minus"></i>
                                </button>
                              )}
                            </div>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                  <br />
                </div>
                <div className="card-footer">
                  <div>
                    <button
                      type="button"
                      onClick={() => handleResetClick()}
                      className="btn btn-warning"
                    >
                      Làm mới
                    </button>
                    <button
                      type="submit"
                      className="btn btn-primary"
                      style={{ float: "right" }}
                    >
                      Thêm mới
                    </button>
                  </div>
                </div>
              </form>
            )}
          </div>
        </TabPanel>

        {props.selectIdClick && Object.keys(props.selectIdClick).length > 0 && (
          <>
            <TabPanel>
              <div className="card card-primary">
                <div className="card-header">
                  <h3 className="card-title">Thông tin bảng giá</h3>
                </div>
                <div>
                  <div className="card-body">
                    <div
                      className="container-datatable"
                      style={{ height: "50vm" }}
                    >
                      <DataTable
                        title="Danh sách bảng giá"
                        columns={columns}
                        data={data}
                        progressPending={IsLoading}
                        pagination
                        paginationServer
                        paginationTotalRows={totalRows}
                        onChangeRowsPerPage={handlePerRowsChange}
                        onChangePage={handlePageChange}
                        highlightOnHover
                      />
                    </div>
                  </div>
                </div>
              </div>
            </TabPanel>
          </>
        )}
      </Tabs>
    </>
  );
};

export default AddPriceTable;
