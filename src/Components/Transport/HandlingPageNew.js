import { Modal } from "bootstrap";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import DatePicker from "react-datepicker";
import moment from "moment";
import { useForm, Controller } from "react-hook-form";
import { useMemo, useState, useEffect, useRef } from "react";
import {
  getData,
  getDataCustom,
  getFilePost,
  postData,
  postFile,
} from "../Common/FuncAxios";
import Cookies from "js-cookie";
import DataTable from "react-data-table-component";
import AddSubFeeByHandling from "./AddSubFeeByHandling";
import ApproveSubFeeByHandling from "./ApproveSubFeeByHandling";
import JoinTransports from "./JoinTransports";
import Select from "react-select";
import { ToastError } from "../Common/FuncToast";
import LoadingPage from "../Common/Loading/LoadingPage";
import HandlingImage from "../FileManager/HandlingImage";
import UpdateHandling from "./UpdateHandling";
import UpdateTransport from "./UpdateTransport";

const HandlingPageNew = () => {
  const accountType = Cookies.get("AccType");
  const Columns = useMemo(() => [
    {
      cell: (val) => (
        <>
          {val.statusId && (
            <>
              <>
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
              </>
              <>{renderButtonByStatus(val)}</>
              <>
                <button
                  onClick={() =>
                    handleEditButtonClick(
                      val,
                      SetShowModal("EditHandling"),
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
                <button
                  onClick={() =>
                    handleEditTransport(
                      val,
                      SetShowModal("EditTransport"),
                      setTitle("Tách Chuyến ")
                    )
                  }
                  type="button"
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Tách Chuyến"
                >
                  <i className="fas fa-sliders-h"></i>
                </button>
              </>
              <>
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
            </>
          )}
        </>
      ),
      width: "220px",
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
      name: <div>CONT NUM</div>,
      selector: (row) => (
        <div style={{ fontWeight: "Bold", fontSize: "16px" }}>
          {row.contNum}
        </div>
      ),
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
      name: <div>Mã Chuyến</div>,
      selector: (row) => <div className="text-wrap">{row.maChuyen}</div>,
      sortable: true,
    },
    {
      name: <div>Mã Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.maVanDonKH}</div>,
      sortable: true,
    },
    {
      name: <div>Loại Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.phanLoaiVanDon}</div>,
      sortable: true,
    },
    accountType &&
      accountType === "NV" &&
      ({
        name: <div>Khách Hàng</div>,
        selector: (row) => <div className="text-wrap">{row.maKH}</div>,
        sortable: true,
      },
      {
        name: <div>Account</div>,
        selector: (row) => <div className="text-wrap">{row.accountName}</div>,
      },
      {
        name: <div>Đơn Vị Vận Tải</div>,
        selector: (row) => <div className="text-wrap">{row.donViVanTai}</div>,
        sortable: true,
      }),
    {
      name: "PTVC",
      selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
      sortable: true,
    },

    {
      name: <div>Điểm Lấy Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.diemDau}</div>,
      sortable: true,
    },
    {
      name: <div>Điểm Trả Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.diemCuoi}</div>,
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
      name: <div>Mã Số Xe</div>,
      selector: (row) => <div className="text-wrap">{row.maSoXe}</div>,
      sortable: true,
    },
    {
      name: <div>Mã CONT</div>,
      selector: (row) => <div className="text-wrap">{row.contNo}</div>,
      sortable: true,
    },
    {
      name: <div>Hãng Tàu</div>,
      selector: (row) => <div className="text-wrap">{row.hangTau}</div>,
      sortable: true,
    },
    // {
    //   name: "Tài Xế",
    //   selector: (row) => <div className="text-wrap">{row.tenTaiXe}</div>,
    //   sortable: true,
    // },
    {
      name: <div>Loại Phương Tiện</div>,
      selector: (row) => row.ptVanChuyen,
      sortable: true,
    },
    {
      name: <div>Khối Lượng</div>,
      selector: (row) => row.khoiLuong,
      sortable: true,
    },
    {
      name: <div>Thể Tích</div>,
      selector: (row) => row.theTich,
      sortable: true,
    },
    {
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

  const {
    control,
    setValue,
    formState: { errors },
  } = useForm({
    mode: "onChange",
  });

  const [loading, setLoading] = useState(true);
  const [ShowConfirm, setShowConfirm] = useState(false);
  const [funcName, setFuncName] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [data, setData] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [page, setPage] = useState(1);
  const [keySearch, setKeySearch] = useState("");
  const [listStatus, setListStatus] = useState([]);
  const [status, setStatus] = useState("");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [listCustomer, setListCustomer] = useState([]);
  const [listCusSelected, setListCusSelected] = useState([]);
  const [listUsers, setListUsers] = useState([]);
  const [listUserSelected, setListUserSelected] = useState([]);
  const [listAccountCus, setListAccountCus] = useState([]);
  const [listAccountSelected, setListAccountSelected] = useState([]);
  const [listSupplier, setListSupplier] = useState([]);
  const [listSupplierSelected, setListSupplierSelected] = useState([]);

  const [selectedRows, setSelectedRows] = useState([]);
  const [toggledClearRows, setToggleClearRows] = useState(false);
  const [itemSelected, setItemSelected] = useState([]);

  const [title, setTitle] = useState("");

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

  const colorStatusText = (text) => {
    let textColor = "";
    switch (text) {
      case "Hoàn Thành":
        textColor = "#69b02a";
        break;
      case "Đã Giao Hàng":
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

  useEffect(() => {
    (async () => {
      const getListAcc = await getData(
        `AccountCustomer/GetListAccountSelectByCus`
      );
      if (getListAcc && getListAcc.length > 0) {
        var obj = [];
        getListAcc.map((val) => {
          obj.push({
            value: val.accountId,
            label: val.accountId + " - " + val.accountName,
          });
        });
        setListAccountCus(obj);
      } else {
        setListAccountCus([]);
      }

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

        let arrSup = [];
        getListCustomer
          .filter((x) => x.loaiKH === "NCC")
          .map((val) => {
            arrSup.push({
              label: val.tenKh,
              value: val.maKh,
            });
          });
        setListSupplier(arrSup);
      }

      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "Handling",
      ]);
      setListStatus(getStatusList);
      fetchData(1);
    })();
  }, []);

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    status = "",
    listCusSelected = [],
    listUserSelected = [],
    listAccountSelected = [],
    listSupplierSelected = []
  ) => {
    fromDate = !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = !toDate ? "" : moment(toDate).format("YYYY-MM-DD");

    let listFilter = {
      customers: listCusSelected,
      users: listUserSelected,
      accountIds: listAccountSelected,
      suppliers: listSupplierSelected,
    };

    const data = await getDataCustom(
      `BillOfLading/GetListHandlingLess?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`,
      listFilter
    );

    setData(data.data);
    setTotalRows(data.totalRecords);
    setLoading(false);
  };

  const handleEditTransport = async (value) => {
    let getTransportById = await getData(
      `BillOfLading/GetTransportById?transportId=${value.maVanDon}`
    );

    setSelectIdClick(getTransportById);
    showModalForm();
  };

  const refeshData = () => {
    fetchData(
      page,
      keySearch,
      fromDate,
      toDate,
      status,
      listCusSelected,
      listUserSelected,
      listAccountSelected,
      listSupplierSelected
    );
  };

  const handlePageChange = async (page) => {
    setPage(page);
    fetchData(
      page,
      keySearch,
      fromDate,
      toDate,
      status,
      listCusSelected,
      listUserSelected,
      listAccountSelected,
      listSupplierSelected
    );
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    let startDate = !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD");
    let endDate = !toDate ? "" : moment(toDate).format("YYYY-MM-DD");

    let listFilter = {
      customers: listCusSelected,
      users: listUserSelected,
      accountIds: listAccountSelected,
      suppliers: listSupplierSelected,
    };

    const data = await getDataCustom(
      `BillOfLading/GetListHandlingLess?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${startDate}&toDate=${endDate}&statusId=${status}`,
      listFilter
    );

    setData(data.data);
    setPerPage(newPerPage);
    setTotalRows(data.totalRecords);
  };

  const renderButtonByStatus = (val) => {
    if (val.ptVanChuyen.includes("CONT")) {
      if (val.phanLoaiVanDon === "xuat") {
        switch (val.statusId) {
          case 27:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Đi Lấy Rỗng"
              >
                <i className="fas fa-cube"></i>
              </button>
            );
          case 17:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Đang Đóng Hàng"
              >
                <i className="fas fa-truck-loading"></i>
              </button>
            );
          case 37:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Đi Vận Chuyển"
              >
                <i className="fas fa-boxes"></i>
              </button>
            );
          case 18:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Đã Giao Hàng"
              >
                <i className="fas fa-clipboard-check"></i>
              </button>
            );
          case 36:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Hoàn Thành"
              >
                <i className="fas fa-check"></i>
              </button>
            );
          default:
            return null;
        }
      }
      if (val.phanLoaiVanDon === "nhap") {
        switch (val.statusId) {
          case 27:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Đi Lấy Hàng"
              >
                <i className="fas fa-trailer"></i>
              </button>
            );
          case 40:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Vận Chuyển Hàng"
              >
                <i className="fas fa-shipping-fast"></i>
              </button>
            );
          case 18:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Đang Giao Hàng"
              >
                <i className="fas fa-truck-loading"></i>
              </button>
            );
          case 41:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Đã Giao Hàng"
              >
                <i className="fas fa-clipboard-check"></i>
              </button>
            );
          case 36:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Đi Trả Rỗng"
              >
                <i className="fas fa-trailer"></i>
              </button>
            );
          case 35:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Đã Trả Rỗng"
              >
                <i className="fas fa-clipboard-check"></i>
              </button>
            );
          case 48:
            return (
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("StartRuning"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Hoàn Thành Chuyến"
              >
                <i className="fas fa-check"></i>
              </button>
            );
          default:
            return null;
        }
      }
    }
    if (val.ptVanChuyen.includes("TRUCK")) {
      switch (val.statusId) {
        case 27:
          return (
            <button
              onClick={() => showConfirmDialog(val, setFuncName("StartRuning"))}
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Đóng Hàng Lên Xe"
            >
              <i className="fas fa-truck-loading"></i>
            </button>
          );
        case 37:
          return (
            <button
              onClick={() => showConfirmDialog(val, setFuncName("StartRuning"))}
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Đi Vận Chuyển Hàng"
            >
              <i className="fas fa-shipping-fast"></i>
            </button>
          );
        case 18:
          return (
            <button
              onClick={() => showConfirmDialog(val, setFuncName("StartRuning"))}
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Đã Giao Hàng"
            >
              <i className="fas fa-clipboard-check"></i>
            </button>
          );
        case 36:
          return (
            <button
              onClick={() => showConfirmDialog(val, setFuncName("StartRuning"))}
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Đã Giao Hàng"
            >
              <i className="fas fa-check"></i>
            </button>
          );
        default:
          return null;
      }
    }
  };

  const showConfirmDialog = (val) => {
    setSelectIdClick(val);
    setShowConfirm(true);
  };

  const handleEditButtonClick = async (value) => {
    setSelectIdClick(value);
    showModalForm();
  };

  const handleSearchClick = () => {
    fetchData(
      1,
      keySearch,
      fromDate,
      toDate,
      status,
      listCusSelected,
      listUserSelected,
      listAccountSelected,
      listSupplierSelected
    );
  };

  const handleOnChangeStatus = (val) => {
    setStatus(val);
    fetchData(
      1,
      keySearch,
      fromDate,
      toDate,
      val,
      listCusSelected,
      listUserSelected,
      listAccountSelected,
      listSupplierSelected
    );
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setStatus("");
    setListCusSelected([]);
    setValue("listCustomers", []);
    setListSupplierSelected([]);
    setValue("listSuppliers", []);
    setListUserSelected([]);
    setValue("listUsers", []);
    setListAccountCus([]);
    setValue("listAccountCus", []);
    fetchData(1);
  };

  const handleClearRows = () => {
    setToggleClearRows(!toggledClearRows);
    setSelectedRows([]);
    setItemSelected([]);
  };

  const handleOnChangeFilterSelect = async (values, type) => {
    if (values) {
      let arrCus = [];
      let arrUsr = [];
      let arrAcc = [];
      let arrSup = [];

      if (type === "suppliers") {
        setValue("listSuppliers", values);

        values.forEach((val) => {
          arrSup.push(val.value);
        });

        setListSupplierSelected(arrSup);
      } else {
        listSupplierSelected.forEach((val) => {
          arrSup.push(val);
        });
      }

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

      if (type === "accountCus") {
        setValue("listAccountCus", values);

        values.forEach((val) => {
          arrAcc.push(val.value);
        });

        setListAccountSelected(arrAcc);
      } else {
        listAccountSelected.forEach((val) => {
          arrAcc.push(val);
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
        page,
        keySearch,
        fromDate,
        toDate,
        status,
        arrCus,
        arrUsr,
        arrAcc,
        arrSup
      );
    }
  };

  const funcAgree = () => {
    if (funcName && funcName.length > 0) {
      switch (funcName) {
        case "StartRuning":
          return ChangeStatusHandling();
        case "CancelHandling":
          return CancelHandling();
      }
    }
  };

  const CancelHandling = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      var cancel = await postData(
        `BillOfLading/cancelHandling?id=${selectIdClick.maDieuPhoi}`
      );
      setShowConfirm(false);
      if (cancel === 1) {
        refeshData();
      }
    }
  };

  const ChangeStatusHandling = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      var update = await postData(
        `BillOfLading/ChangeStatusHandling?id=${selectIdClick.maDieuPhoi}&maChuyen=${selectIdClick.maChuyen}`
      );
      setShowConfirm(false);
      if (update === 1) {
        refeshData();
      }
    }
  };

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const handleOnClickMarge = () => {
    if (selectedRows && selectedRows.length > 0) {
      setItemSelected(selectedRows);
      showModalForm(SetShowModal("MargeTransport"), setTitle("Gộp Chuyến"));
    } else {
      setItemSelected([]);
      ToastError("Vui lòng chọn vận đơn để gộp");
      return;
    }
  };

  const handleExportExcel = async () => {
    if (!fromDate && !toDate) {
      ToastError("Vui lòng chọn mốc thời gian");
      return;
    }

    setLoading(true);

    let startDate = moment(fromDate).format("YYYY-MM-DD");
    let endDate = moment(toDate).format("YYYY-MM-DD");

    let listFilter = {
      customers: listCusSelected,
      users: listUserSelected,
      accountIds: listAccountSelected,
      suppliers: listSupplierSelected,
    };

    const getFileDownLoad = await getFilePost(
      `BillOfLading/ExportExcelHandlingLess?KeyWord=${keySearch}&fromDate=${startDate}&toDate=${endDate}&statusId=${status}`,
      listFilter,
      "DieuPhoi" + moment(new Date()).format("DD/MM/YYYY HHmmss")
    );
    setLoading(false);
  };

  const sendMailToSupplier = async () => {
    if (selectedRows && selectedRows.length > 0) {
      setItemSelected(selectedRows);
      let handlingIds = [];

      selectedRows.forEach((element) => {
        handlingIds.push(parseInt(element.maDieuPhoi));
      });

      let sendMail = await postData("BillOfLading/SendMailToSupplier", {
        ids: handlingIds,
      });

      handleClearRows();
    } else {
      handleClearRows();
      ToastError("Vui lòng chọn chuyến để gửi mail");
      return;
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
        <div className="card card-default">
          <div className="card-header">
            <div className="container-fruid">
              <div className="row">
                {accountType && accountType === "NV" && (
                  <>
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
                      <button
                        type="button"
                        className="btn btn-title btn-sm btn-default mx-1"
                        gloss="Gộp Chuyến "
                        onClick={() => handleOnClickMarge()}
                      >
                        <i className="fas fa-layer-group"></i>
                      </button>
                      <button
                        type="button"
                        className="btn btn-title btn-sm btn-default mx-1"
                        gloss="Gửi Mail NCC "
                        onClick={() => sendMailToSupplier()}
                      >
                        <i className="fas fa-mail-bulk"></i>
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
                          name="listSuppliers"
                          control={control}
                          render={({ field }) => (
                            <Select
                              {...field}
                              className="basic-multi-select"
                              classNamePrefix={"form-control"}
                              isMulti
                              value={field.value}
                              options={listSupplier}
                              styles={customStyles}
                              onChange={(field) =>
                                handleOnChangeFilterSelect(field, "suppliers")
                              }
                              placeholder="Chọn NCC"
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
                      <div className="form-group">
                        <Controller
                          name="listAccountCus"
                          control={control}
                          render={({ field }) => (
                            <Select
                              {...field}
                              className="basic-multi-select"
                              classNamePrefix={"form-control"}
                              isMulti
                              value={field.value}
                              options={listAccountCus}
                              styles={customStyles}
                              onChange={(field) =>
                                handleOnChangeFilterSelect(field, "accountCus")
                              }
                              placeholder="Chọn Account"
                            />
                          )}
                        />
                      </div>
                    </div>
                  </>
                )}

                <div className="col col-sm">
                  <div className="row">
                    <div className="col col-sm">
                      <div className="input-group input-group-sm">
                        <select
                          className="form-control form-control-sm"
                          onChange={(e) => handleOnChangeStatus(e.target.value)}
                          value={status}
                        >
                          <option value="">Tất Cả Trạng Thái</option>
                          <option value={"null"}>Mới Tạo</option>
                          {listStatus &&
                            listStatus.map((val) => {
                              return (
                                <option value={val.statusId} key={val.statusId}>
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
                      placeholder="Tìm kiếm"
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
                title="Danh sách vận đơn"
                direction="auto"
                responsive
                columns={Columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                paginationRowsPerPageOptions={[10, 30, 50, 100]}
                paginationTotalRows={totalRows}
                onSelectedRowsChange={handleChange}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                clearSelectedRows={toggledClearRows}
                selectableRows
                highlightOnHover
                striped
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
      )}

      <div>
        {ShowConfirm === true && (
          <ConfirmDialog
            setShowConfirm={setShowConfirm}
            title={"Bạn có chắc chắn với quyết định này?"}
            content={
              "Khi thực hiện hành động này sẽ không thể hoàn tác lại được nữa."
            }
            // funcAgree={funcAgree}
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
                  {ShowModal === "MargeTransport" &&
                    selectIdClick &&
                    itemSelected.length > 0 && (
                      <JoinTransports
                        getListTransport={refeshData}
                        items={itemSelected}
                        clearItems={handleClearRows}
                        hideModal={hideModal}
                      />
                    )}
                  {ShowModal === "EditTransport" && (
                    <UpdateTransport
                      getListTransport={refeshData}
                      selectIdClick={selectIdClick}
                      hideModal={hideModal}
                    />
                  )}
                  {ShowModal === "EditHandling" && (
                    <>
                      {!selectIdClick.tongVanDonGhep && (
                        <>
                          <UpdateHandling
                            getlistData={refeshData}
                            selectIdClick={selectIdClick}
                            hideModal={hideModal}
                          />
                        </>
                      )}
                      {selectIdClick.tongVanDonGhep && (
                        <>
                          <JoinTransports
                            clearItems={handleClearRows}
                            getListTransport={refeshData}
                            selectIdClick={selectIdClick}
                            items={[]}
                            hideModal={hideModal}
                          />
                        </>
                      )}
                    </>
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
    </>
  );
};
export default HandlingPageNew;
