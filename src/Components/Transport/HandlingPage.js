import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import {
  getData,
  postData,
  postFile,
  getDataCustom,
  getFilePost,
} from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import UpdateHandling from "./UpdateHandling";
import Select from "react-select";
import HandlingImage from "../FileManager/HandlingImage";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import AddSubFeeByHandling from "./AddSubFeeByHandling";
import ApproveSubFeeByHandling from "./ApproveSubFeeByHandling";
import { ToastError } from "../Common/FuncToast";
import LoadingPage from "../Common/Loading/LoadingPage";

const HandlingPage = (props) => {
  const { dataClick } = props;

  const {
    control,
    setValue,
    formState: { errors },
  } = useForm({
    mode: "onChange",
  });

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);

  const [ShowConfirm, setShowConfirm] = useState(false);
  const [funcName, setFuncName] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();
  const [selectIdClick, setSelectIdClick] = useState({});

  const [totalRows, setTotalRows] = useState(0);
  const [selectedRows, setSelectedRows] = useState([]);
  const [perPage, setPerPage] = useState(10);
  const [page, setPage] = useState(1);
  const [keySearch, setKeySearch] = useState("");
  const [listStatus, setListStatus] = useState([]);
  const [status, setStatus] = useState("");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [transportId, setTransportId] = useState("");
  const [listCustomer, setListCustomer] = useState([]);
  const [listCusSelected, setListCusSelected] = useState([]);
  const [listUsers, setListUsers] = useState([]);
  const [listUserSelected, setListUserSelected] = useState([]);

  const [title, setTitle] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <div>
          {/* <>
            {val.statusId === 19 ? (
              <>
                <button
                  onClick={() =>
                    showConfirmDialog(val, setFuncName("CloneHandling"))
                  }
                  type="button"
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Sao Chép"
                >
                  <i className="far fa-clone"></i>
                </button>
                <button
                  onClick={() =>
                    showConfirmDialog(val, setFuncName("RemoveHandling"))
                  }
                  type="button"
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Xóa"
                >
                  <i className="fas fa-trash-alt"></i>
                </button>
              </>
            ) : (
              <span></span>
            )}
          </> */}
          <>
            {val.statusId === 27 || val.statusId === 19 ? (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("CancelHandling"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Hủy Chuyến"
              >
                <i className="fas fa-window-close"></i>
              </button>
            ) : (
              <span></span>
            )}
          </>
          <>{renderButton(val)}</>
          <>
            <button
              onClick={() =>
                handleEditButtonClick(
                  val,
                  SetShowModal("Edit"),
                  setTitle("Cập Nhật Thông Tin Điều Phối")
                )
              }
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Chỉnh Sửa"
            >
              <i className="far fa-edit"></i>
            </button>
          </>
          <>
            {val.statusId !== 20 || val.statusId !== 21 || val.statusId !== 31}
            <button
              onClick={() =>
                handleEditButtonClick(
                  val,
                  SetShowModal("addSubFee"),
                  setTitle("Thêm Mới Phụ Phí Phát Sinh")
                )
              }
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Phụ Phí Phát Sinh"
            >
              <i className="fas fa-file-invoice-dollar"></i>
            </button>
          </>
          <>
            <button
              onClick={() =>
                handleEditButtonClick(
                  val,
                  SetShowModal("Image"),
                  setTitle("Quản Lý Chứng Từ Chuyến")
                )
              }
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Xem Hình Ảnh"
            >
              <i className="fas fa-image"></i>
            </button>
          </>
          <>
            <div
              className="upload-btn-wrapper mx-1 btn-title"
              gloss="Upload Hình Ảnh"
            >
              <button className="btn btn-sm btn-default mx-1">
                <i className="fas fa-file-upload"></i>
              </button>
              <input
                type="file"
                name="myfile"
                multiple
                accept="image/png, image/jpg, image/jpeg"
                onChange={(e) => handleUploadImage(val, e)}
              />
            </div>
          </>
        </div>
      ),
      width: "250px",
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: <div>Trạng Thái</div>,
      selector: (row) => (
        <div className="text-wrap">{colorStatusText(row.trangThai)}</div>
      ),
      sortable: true,
    },
    {
      selector: (row) => row.maDieuPhoi,
      omit: true,
    },
    {
      selector: (row) => row.maVanDon,
      omit: true,
    },
    {
      name: <div>CONT NUM</div>,
      selector: (row) => (
        <div style={{ fontWeight: "Bold", fontSize: "16px" }}>
          {row.contNum}
        </div>
      ),
    },
    {
      name: <div>Booking No</div>,
      selector: (row) => <div className="text-wrap">{row.maVanDonKH}</div>,
      sortable: true,
    },
    {
      name: <div>Loại Vận Đơn</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.phanLoaiVanDon === "xuat" ? "XUẤT" : "NHẬP"}
        </div>
      ),
      sortable: true,
    },
    {
      name: "PTVC",
      selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
      sortable: true,
    },
    {
      name: <div>Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.maKH}</div>,
      sortable: true,
    },
    {
      name: <div>Mã CONT</div>,
      selector: (row) => <div className="text-wrap">{row.contNo}</div>,
      sortable: true,
    },
    {
      name: <div>Loại Phương Tiện</div>,
      selector: (row) => row.ptVanChuyen,
      sortable: true,
    },
    {
      name: <div>Hãng Tàu</div>,
      selector: (row) => <div className="text-wrap">{row.hangTau}</div>,
      sortable: true,
    },
    {
      name: <div>Điểm Lấy Rỗng</div>,
      selector: (row) => <div className="text-wrap">{row.diemLayRong}</div>,
      sortable: true,
    },
    {
      name: <div>Điểm Trả Rỗng</div>,
      selector: (row) => <div className="text-wrap">{row.diemTraRong}</div>,
      sortable: true,
    },
    {
      name: <div>Đơn Vị Vận Tải</div>,
      selector: (row) => <div className="text-wrap">{row.donViVanTai}</div>,
      sortable: true,
    },

    {
      name: <div>Điểm Đóng Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.diemDau}</div>,
      sortable: true,
    },
    {
      name: <div>Điểm Hạ Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.diemCuoi}</div>,
      sortable: true,
    },

    // {
    //   name: <div>Mã Số Xe</div>,
    //   selector: (row) => <div className="text-wrap">{row.maSoXe}</div>,
    //   sortable: true,
    // },

    {
      name: <div>Khối Lượng</div>,
      selector: (row) => row.khoiLuong,
      sortable: true,
    },
    {
      name: <div>Số Khối</div>,
      selector: (row) => row.theTich,
      sortable: true,
    },

    {
      name: "statusId",
      selector: (row) => row.statusId,
      omit: true,
    },
    {
      name: <div>Thời Gian Tạo</div>,
      selector: (row) => (
        <div className="text-wrap">
          {moment(row.thoiGianTaoDon).format("DD/MM/YYYY HH:mm:ss")}
        </div>
      ),
      sortable: true,
      grow: 2,
    },
  ]);

  useEffect(() => {
    if (props && dataClick && Object.keys(dataClick).length > 0) {
      setTransportId(dataClick.maVanDon);
      fetchData(dataClick.maVanDon, 1);
    }
  }, [props, dataClick, listStatus]);

  useEffect(() => {
    (async () => {
      let getlistUser = await getData(`Common/GetListUser`);
      if (getlistUser && getlistUser.length > 0) {
        let arrUser = [];
        getlistUser.forEach((val) => {
          arrUser.push({ label: val.name, value: val.userName });
        });

        setListUsers(arrUser);
      }

      let getListCustomer = await getData(`Customer/GetListCustomerFilter`);
      if (getListCustomer && getListCustomer.length > 0) {
        let arrKh = [];
        getListCustomer
          .filter((x) => x.loaiKH === "KH")
          .map((val) => {
            arrKh.push({
              label: val.tenKh,
              value: val.maKh,
            });
          });
        setListCustomer(arrKh);
      }

      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "Handling",
      ]);
      setListStatus(getStatusList);
      setTransportId("");
      fetchData("", 1);
    })();
  }, []);

  const colorStatusText = (text) => {
    let textColor = "";
    switch (text) {
      case "Hoàn Thành":
        textColor = "#69b02a";
        break;
      case "Không Duyệt":
        textColor = "#ef4130";
        break;
      case "Chờ Duyệt":
        textColor = "#4ac4d3";
        break;
      case "Đã Hủy":
        textColor = "#ef4130";
        break;
      case "Chờ Điều Phối":
        textColor = "#272a64";
        break;
      case "Đang Vận Chuyển":
        textColor = "#f90";
        break;
      case "Đang Lấy Rỗng":
        textColor = "#f90";
        break;
      case "Đang Trả Rỗng":
        textColor = "#f90";
        break;
      case "Chờ Vận Chuyển":
        textColor = "#063970";
        break;
      default:
        textColor = "????";
    }
    return <p style={{ color: textColor, fontWeight: "bold" }}>{text}</p>;
  };

  const renderButton = (val) => {
    switch (val.statusId) {
      case 27:
        return val.ptVanChuyen.includes("CONT") &&
          val.phanLoaiVanDon === "xuat" ? (
          <button
            title="Đi Lấy Rỗng"
            onClick={() => showConfirmDialog(val, setFuncName("StartRuning"))}
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Đi Lấy Rỗng"
          >
            <i className="fas fa-cube"></i>
          </button>
        ) : (
          <button
            title="Đi Giao Hàng"
            onClick={() => showConfirmDialog(val, setFuncName("StartRuning"))}
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Đi Giao Hàng"
          >
            <i className="fas fa-shipping-fast"></i>
          </button>
        );
      case 17:
        return (
          <button
            title="Đi Giao Hàng"
            onClick={() => showConfirmDialog(val, setFuncName("StartRuning"))}
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Đi Giao Hàng"
          >
            <i className="fas fa-shipping-fast"></i>
          </button>
        );
      case 18:
        return val.ptVanChuyen.includes("CONT") &&
          val.phanLoaiVanDon === "nhap" ? (
          <button
            title="Đi Trả Rỗng"
            onClick={() => showConfirmDialog(val, setFuncName("StartRuning"))}
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Đi Trả Rỗng"
          >
            <i className="fas fa-cube"></i>
          </button>
        ) : (
          <button
            title="Hoàn Thành Chuyến"
            onClick={() => showConfirmDialog(val, setFuncName("StartRuning"))}
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Hoàn Thành Chuyến"
          >
            <i className="fas fa-check"></i>
          </button>
        );
      case 35:
        return (
          <button
            title="Hoàn Thành Chuyến"
            onClick={() => showConfirmDialog(val, setFuncName("StartRuning"))}
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Hoàn Thành Chuyến"
          >
            <i className="fas fa-check"></i>
          </button>
        );
      case 21:
        return (
          <>
            <span>
              <i className="fas fa-window-close mx-1"></i>
            </span>
          </>
        );
      case 20:
        return (
          <>
            <span>
              <i className="fas fa-check mx-1"></i>
            </span>
          </>
        );
      default:
        return null;
    }
  };

  const fetchData = async (
    transportId,
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    status = "",
    listCusSelected = [],
    listUsrSelected = []
  ) => {
    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");

    let listFilter = {
      customers: listCusSelected,
      users: listUsrSelected,
    };

    const dataCus = await getDataCustom(
      `BillOfLading/GetListHandling?transportId=${transportId}&PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`,
      listFilter
    );

    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (newPage) => {
    fetchData(
      transportId,
      newPage,
      keySearch,
      fromDate,
      toDate,
      status,
      listCusSelected,
      listUserSelected
    );
    setPage(newPage);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    if (newPerPage !== perPage) {
      let listFilter = {
        customers: listCusSelected,
        users: listUserSelected,
      };

      const dataCus = await getDataCustom(
        `BillOfLading/GetListHandling?transportId=${transportId}&PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`,
        listFilter
      );
      setPerPage(newPerPage);
      setData(dataCus.data);
      setTotalRows(dataCus.totalRecords);
    }
  };

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const handleUploadImage = async (val, e) => {
    let files = e.target.files;
    const transportId = val.maVanDon;
    const handlingId = val.maDieuPhoi;

    let arrfiles = [];

    for (let i = 0; i <= files.length - 1; i++) {
      arrfiles.push(files[i]);
    }
    const uploadFiles = await postFile("BillOfLading/UploadFile", {
      files: arrfiles,
      transportId: transportId,
      handlingId: handlingId,
    });
  };

  const showConfirmDialog = (val) => {
    setSelectIdClick(val);
    setShowConfirm(true);
  };

  const setRuning = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      var update = await postData(
        `BillOfLading/SetRuning?id=${selectIdClick.maDieuPhoi}`
      );

      if (update === 1) {
        await fetchData(
          transportId,
          page,
          keySearch,
          fromDate,
          toDate,
          status,
          listCusSelected,
          listUserSelected
        );
        setShowConfirm(false);
      } else {
        setShowConfirm(false);
      }
    }
  };

  const setCancelHandling = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      var update = await postData(
        `BillOfLading/CancelHandling?id=${selectIdClick.maDieuPhoi}`
      );

      if (update === 1) {
        await fetchData(
          transportId,
          page,
          keySearch,
          fromDate,
          toDate,
          status,
          listCusSelected,
          listUserSelected
        );
        setShowConfirm(false);
      } else {
        setShowConfirm(false);
      }
    }
  };

  const funcAgree = async () => {
    if (funcName && funcName.length > 0) {
      switch (funcName) {
        case "StartRuning":
          return await setRuning();
        case "CancelHandling":
          return await setCancelHandling();
      }
    }
  };

  const handleEditButtonClick = (value) => {
    setSelectIdClick(value);
    showModalForm();
  };

  const handleSearchClick = () => {
    fetchData(
      transportId,
      page,
      keySearch,
      fromDate,
      toDate,
      status,
      listCusSelected,
      listUserSelected
    );
  };

  const handleOnChangeStatus = (value) => {
    setStatus(value);
    fetchData(
      transportId,
      page,
      keySearch,
      fromDate,
      toDate,
      value,
      listCusSelected,
      listUserSelected
    );
  };

  const handleRefeshDataClick = () => {
    setPage(1);
    setPerPage(10);
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setStatus("");
    setListCusSelected([]);
    setValue("listCustomers", []);
    setListUserSelected([]);
    setValue("listUsers", []);
    fetchData("", 1);
  };

  const refeshData = () => {
    fetchData(
      transportId,
      page,
      keySearch,
      fromDate,
      toDate,
      status,
      listCusSelected,
      listUserSelected
    );
  };

  const showModalForm = () => {
    const modal = new Modal(parseExceptionModal.current, {
      keyboard: false,
      backdrop: "static",
    });
    setModal(modal);
    modal.show();
  };

  const hideModal = () => {
    modal.hide();
  };

  const handleExportExcel = async () => {
    if (!fromDate || !toDate) {
      ToastError("Vui lòng chọn mốc thời gian");
      return;
    }

    let startDate = moment(fromDate).format("YYYY-MM-DD");
    let endDate = moment(toDate).format("YYYY-MM-DD");

    let listFilter = {
      customers: listCusSelected,
      users: listUserSelected,
    };

    const getFileDownLoad = await getFilePost(
      `BillOfLading/ExportExcelHandLing?KeyWord=${keySearch}&fromDate=${startDate}&toDate=${endDate}&statusId=${status}`,
      listFilter,
      "DieuPhoi" + moment(new Date()).format("DD/MM/YYYY HHmmss")
    );
  };

  const handleOnChangeFilterSelect = async (values, type) => {
    if (values) {
      setLoading(true);

      let arrCus = [];
      let arrUsr = [];

      if (type === "customers") {
        setValue("listCustomers", values);

        values.forEach((val) => {
          arrCus.push(val.value);
        });

        setListCusSelected(arrCus);
      } else {
        listCusSelected.forEach((val) => {
          arrCus.push(val);
        });
      }

      if (type === "users") {
        setValue("listUsers", values);

        values.forEach((val) => {
          arrUsr.push(val.value);
        });
        setListUserSelected(arrUsr);
      } else {
        listUserSelected.forEach((val) => {
          arrUsr.push(val);
        });
      }

      await fetchData(
        transportId,
        page,
        keySearch,
        fromDate,
        toDate,
        status,
        arrCus,
        arrUsr
      );
      setLoading(false);
    }
  };

  const customStyles = {
    control: (provided, state) => ({
      ...provided,
      background: "#fff",
      borderColor: "#9e9e9e",
      minHeight: "30px",
      height: "30px",
      boxShadow: state.isFocused ? null : null,
    }),

    valueContainer: (provided, state) => ({
      ...provided,
      height: "30px",
      padding: "0 6px",
    }),

    input: (provided, state) => ({
      ...provided,
      margin: "0px",
    }),
    indicatorSeparator: (state) => ({
      display: "none",
    }),
    indicatorsContainer: (provided, state) => ({
      ...provided,
      height: "30px",
    }),
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản Lý Điều Phối</h1>
            </div>
          </div>
        </div>
      </section>

      {loading === true ? (
        <div>
          <LoadingPage></LoadingPage>
        </div>
      ) : (
        <section className="content">
          <div className="card">
            <div className="card-header">
              <div className="container-fruid">
                <div className="row">
                  <div className="col col-sm">
                    <button
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Duyệt Phụ Phí"
                      type="button"
                      onClick={() =>
                        showModalForm(
                          SetShowModal("ApproveSubFee"),
                          setSelectIdClick({}),
                          setTitle("Duyệt Phụ Phí Phát Sinh")
                        )
                      }
                    >
                      <i className="fas fa-check-double"></i>
                    </button>
                  </div>
                  <div className="col col-sm">
                    <div className="form-group">
                      <Controller
                        name="listUsers"
                        control={control}
                        render={({ field }) => (
                          <Select
                            {...field}
                            className="basic-multi-select"
                            classNamePrefix={"form-control"}
                            isMulti
                            value={field.value}
                            options={listUsers}
                            styles={customStyles}
                            onChange={(field) =>
                              handleOnChangeFilterSelect(field, "users")
                            }
                            placeholder="Chọn Users"
                          />
                        )}
                      />
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="form-group">
                      <Controller
                        name="listCustomers"
                        control={control}
                        render={({ field }) => (
                          <Select
                            {...field}
                            className="basic-multi-select"
                            classNamePrefix={"form-control"}
                            isMulti
                            value={field.value}
                            options={listCustomer}
                            styles={customStyles}
                            onChange={(field) =>
                              handleOnChangeFilterSelect(field, "customers")
                            }
                            placeholder="Chọn Khách Hàng"
                          />
                        )}
                      />
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="row">
                      <div className="col col-sm">
                        <div className="input-group input-group-sm">
                          <select
                            className="form-control form-control-sm"
                            onChange={(e) =>
                              handleOnChangeStatus(e.target.value)
                            }
                            value={status}
                          >
                            <option value="">Tất Cả Trạng Thái</option>
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
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="row">
                      <div className="col col-sm">
                        <div className="input-group input-group-sm">
                          <DatePicker
                            selected={fromDate}
                            onChange={(date) => setFromDate(date)}
                            dateFormat="dd/MM/yyyy"
                            className="form-control form-control-sm"
                            placeholderText="Từ ngày"
                            value={fromDate}
                            showMonthDropdown
                            showYearDropdown
                            dropdownMode="select"
                          />
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="input-group input-group-sm">
                          <DatePicker
                            selected={toDate}
                            onChange={(date) => setToDate(date)}
                            dateFormat="dd/MM/yyyy"
                            className="form-control form-control-sm"
                            placeholderText="Đến Ngày"
                            value={toDate}
                          />
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="col col-sm ">
                    <div className="input-group input-group-sm">
                      <input
                        type="text"
                        className="form-control"
                        value={keySearch}
                        onChange={(e) => setKeySearch(e.target.value)}
                      />
                      <span className="input-group-append">
                        <button
                          type="button"
                          className="btn btn-sm btn-default"
                          onClick={() => handleSearchClick()}
                        >
                          <i className="fas fa-search"></i>
                        </button>
                      </span>
                      <button
                        type="button"
                        className="btn btn-sm btn-default mx-2"
                        onClick={() => handleRefeshDataClick()}
                      >
                        <i className="fas fa-sync-alt"></i>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div className="card-body">
              <div className="container-datatable" style={{ height: "50vm" }}>
                <DataTable
                  columns={columns}
                  data={data}
                  progressPending={loading}
                  pagination
                  paginationServer
                  paginationRowsPerPageOptions={[10, 30, 50, 100]}
                  paginationTotalRows={totalRows}
                  onSelectedRowsChange={handleChange}
                  onChangeRowsPerPage={handlePerRowsChange}
                  onChangePage={handlePageChange}
                  highlightOnHover
                  striped
                  direction="auto"
                  dense
                  responsive
                  fixedHeader
                  fixedHeaderScrollHeight="60vh"
                />
              </div>
            </div>
            <div className="card-footer">
              <div className="row">
                <div className="col-sm-3">
                  <button
                    // href={FileExcelImport}
                    onClick={() => handleExportExcel()}
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Tải File Excel"
                    type="button"
                  >
                    <i className="fas fa-file-excel"></i>
                  </button>
                  {/* <div className="upload-btn-wrapper">
                  <button className="btn btn-sm btn-default mx-1">
                    <i className="fas fa-upload"></i>
                  </button>
                  <input
                    type="file"
                    name="myfile"
                    // onChange={(e) => handleExcelImportClick(e)}
                  />
                </div> */}
                </div>
              </div>
            </div>
          </div>
          <div>
            {ShowConfirm === true && (
              <ConfirmDialog
                setShowConfirm={setShowConfirm}
                title={"Bạn có chắc chắn với quyết định này?"}
                content={
                  "Khi thực hiện hành động này sẽ không thể hoàn tác lại được nữa."
                }
                funcAgree={funcAgree}
              />
            )}
            <div
              className="modal fade"
              id="modal-xl"
              data-backdrop="static"
              ref={parseExceptionModal}
              aria-labelledby="parseExceptionModal"
              backdrop="static"
            >
              <div
                className="modal-dialog modal-dialog-scrollable"
                style={{ maxWidth: "95%" }}
              >
                <div className="modal-content">
                  <div className="modal-header">
                    <h5>{title}</h5>
                    <button
                      type="button"
                      className="close"
                      data-dismiss="modal"
                      onClick={() => hideModal()}
                      aria-label="Close"
                    >
                      <span aria-hidden="true">×</span>
                    </button>
                  </div>
                  <div className="modal-body">
                    <>
                      {ShowModal === "Edit" && (
                        <UpdateHandling
                          getlistData={refeshData}
                          selectIdClick={selectIdClick}
                          hideModal={hideModal}
                        />
                      )}
                      {ShowModal === "Image" && (
                        <HandlingImage
                          dataClick={selectIdClick}
                          hideModal={hideModal}
                          checkModal={modal}
                        />
                      )}
                      {ShowModal === "addSubFee" && (
                        <AddSubFeeByHandling dataClick={selectIdClick} />
                      )}
                      {ShowModal === "ApproveSubFee" && (
                        <ApproveSubFeeByHandling CheckModalShow={modal} />
                      )}
                    </>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>
      )}
    </>
  );
};

export default HandlingPage;
