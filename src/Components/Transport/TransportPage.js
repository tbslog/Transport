import { useMemo, useState, useEffect, useRef } from "react";
import { getData, getDataCustom, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import CreateTransport from "./CreateTransport";
import UpdateTransport from "./UpdateTransport";
import HandlingPage from "./HandlingPage";
import CreateTransportLess from "./CreateTransportLess";
import UpdateTransportLess from "./UpdateTransportLess";
import Select from "react-select";
import HandlingByTransport from "./HandlingByTransport";
import Cookies from "js-cookie";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import LoadingPage from "../Common/Loading/LoadingPage";

const TransportPage = () => {
  const {
    control,
    setValue,
    formState: { errors },
  } = useForm({
    mode: "onChange",
  });

  const accountType = Cookies.get("AccType");

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [page, setPage] = useState(1);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [selectIdClick, setSelectIdClick] = useState({});

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [listStatus, setListStatus] = useState([]);
  const [status, setStatus] = useState("");
  const [maPTVC, setMaPTVC] = useState("");
  const [listCustomer, setListCustomer] = useState([]);
  const [listCusSelected, setListCusSelected] = useState([]);
  const [listUsers, setListUsers] = useState([]);
  const [listUsersSelected, setListUsersSelected] = useState([]);

  const [selectedRows, setSelectedRows] = useState([]);
  const [toggledClearRows, setToggleClearRows] = useState(false);
  const [itemSelected, setItemSelected] = useState([]);

  const [ShowConfirm, setShowConfirm] = useState(false);
  const [funcName, setFuncName] = useState("");

  const [title, setTitle] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <>
          {accountType && accountType === "NV" && val.maTrangThai === 28 && (
            <>
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("RejecteTransport"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Từ Chối"
              >
                <i className="fas fa-minus-circle"></i>
              </button>
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("AcceptTransport"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Chấp Nhận"
              >
                <i className="fas fa-check-circle"></i>
              </button>
            </>
          )}
          {((accountType && accountType === "KH" && val.maTrangThai === 28) ||
            (accountType && accountType === "NV")) && (
            <button
              onClick={() =>
                handleEditButtonClick(
                  val,
                  val.maPTVC === "LCL" || val.maPTVC === "LTL"
                    ? SetShowModal("EditLess")
                    : SetShowModal("Edit"),
                  setTitle("Cập Nhật Thông Tin Vận Đơn")
                )
              }
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Chỉnh Sửa"
            >
              <i className="far fa-edit"></i>
            </button>
          )}
          <button
            onClick={() =>
              handleEditButtonClick(
                val,
                SetShowModal("ShowListHandling"),
                setTitle("Danh Sách Chuyến Của Vận Đơn")
              )
            }
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Xem DS Chuyến"
          >
            <i className="fas fa-list-ol"></i>
          </button>
        </>
      ),
      width: "200px",
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      selector: (row) => <div className="text-wrap">{row.maVanDon}</div>,
      omit: true,
    },

    {
      name: <div>Mã Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.maVanDonKH}</div>,
    },
    {
      name: <div>Loại Vận Đơn</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.loaiVanDon === "xuat" ? "XUẤT" : "NHẬP"}
        </div>
      ),
    },
    {
      name: <div>PTVC</div>,
      selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
    },
    {
      name: <div>Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.tenKH}</div>,
    },

    {
      name: <div>Mã Cung Đường</div>,
      selector: (row) => row.maCungDuong,
      sortable: true,
      omit: true,
    },
    {
      name: <div>Tên Cung Đường</div>,
      selector: (row) => <div className="text-wrap">{row.tenCungDuong}</div>,
      sortable: true,
    },
    {
      name: <div>Tổng Trọng Lượng</div>,
      selector: (row) => row.tongKhoiLuong,
      sortable: true,
      Cell: ({ row }) => <div className="text-wrap">{row.tongKhoiLuong}</div>,
    },
    {
      name: <div>Tổng Thể Tích</div>,
      selector: (row) => row.tongTheTich,
      sortable: true,
    },
    {
      name: <div>Tổng Số Kiện</div>,
      selector: (row) => row.tongSoKien,
      sortable: true,
      Cell: ({ row }) => <div className="text-wrap">{row.tongSoKhoi}</div>,
    },
    {
      name: <div>Thời Gian Lấy/Trả Rỗng</div>,
      selector: (row) =>
        !row.thoiGianLayTraRong ? null : (
          <div className="text-wrap">
            {moment(row.thoiGianLayTraRong).format("DD/MM/YYYY HH:mm")}
          </div>
        ),
      sortable: true,
    },
    {
      name: <div>Thời Gian Hạn Lệnh</div>,
      selector: (row) =>
        !row.thoiGianHanLenh ? null : (
          <div className="text-wrap">
            {moment(row.thoiGianHanLenh).format("DD/MM/YYYY HH:mm")}
          </div>
        ),
      sortable: true,
    },
    {
      name: <div>Thời Gian Hạ Cảng</div>,
      selector: (row) =>
        !row.thoiGianHaCang ? null : (
          <div className="text-wrap">
            {moment(row.thoiGianHaCang).format("DD/MM/YYYY HH:mm")}
          </div>
        ),
      sortable: true,
    },
    {
      selector: (row) => row.maTrangThai,
      sortable: true,
      omit: true,
    },
    {
      name: <div>Trạng Thái</div>,
      selector: (row) => (
        <div className="text-wrap">{colorStatusText(row.trangThai)}</div>
      ),
      sortable: true,
    },
    {
      name: <div>Thời Gian Lập Đơn</div>,

      selector: (row) => (
        <div className="text-wrap">
          {moment(row.thoiGianTaoDon).format("DD/MM/YYYY HH:mm")}
        </div>
      ),
      sortable: true,
    },
  ]);

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
      case "Chờ Vận Chuyển":
        textColor = "#063970";
        break;
      default:
        textColor = "????";
    }
    return <p style={{ color: textColor, fontWeight: "bold" }}>{text}</p>;
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
        "Transport",
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
    maptvc = "",
    listCusSelected = [],
    listUsersSelected = []
  ) => {
    setLoading(true);

    let listFilter = {
      customers: listCusSelected,
      users: listUsersSelected,
    };
    const datatransport = await getDataCustom(
      `BillOfLading/GetListTransport?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&StatusId=${status}&fromDate=${fromDate}&toDate=${toDate}&maptvc=${maptvc}`,
      listFilter
    );

    setData(datatransport.data);
    setTotalRows(datatransport.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    setPage(page);
    fetchData(
      page,
      keySearch,
      !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD"),
      !toDate ? "" : moment(toDate).format("YYYY-MM-DD"),
      status,
      maPTVC,
      listCusSelected,
      listUsersSelected
    );
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    let listFilter = {
      customers: listCusSelected,
      users: listUsersSelected,
    };

    const datatransport = await getDataCustom(
      `BillOfLading/GetListTransport?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&StatusId=${status}&fromDate=${fromDate}&toDate=${toDate}&maptvc=${maPTVC}`,
      listFilter
    );
    setData(datatransport.data);
    setPerPage(newPerPage);
    setLoading(false);
  };

  const handleEditButtonClick = async (value) => {
    let getTransportById = await getData(
      `BillOfLading/GetTransportById?transportId=${value.maVanDon}`
    );

    setSelectIdClick(getTransportById);
    showModalForm();
  };

  const funcAgree = () => {
    if (funcName && funcName.length > 0) {
      switch (funcName) {
        case "RejecteTransport":
          return AcceptOrRejectTransport(1);
        case "AcceptTransport":
          return AcceptOrRejectTransport(0);
      }
    }
  };

  const showConfirmDialog = (val) => {
    setSelectIdClick(val);
    setShowConfirm(true);
  };

  const AcceptOrRejectTransport = async (val) => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      var update = await postData(
        `BillOfLading/AcceptOrRejectTransport?transportId=${selectIdClick.maVanDon}&action=${val}`
      );

      if (update === 1) {
        fetchData(
          page,
          keySearch,
          fromDate,
          toDate,
          status,
          maPTVC,
          listCusSelected,
          listUsersSelected
        );
        setShowConfirm(false);
      } else {
        setShowConfirm(false);
      }
    }
  };

  const handleSearchClick = () => {
    fetchData(
      1,
      keySearch,
      !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD"),
      !toDate ? "" : moment(toDate).format("YYYY-MM-DD"),
      status,
      maPTVC,
      listCusSelected,
      listUsersSelected
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
      maPTVC,
      listCusSelected,
      listUsersSelected
    );
  };

  const handleOnChangeMaPTVC = (val) => {
    setMaPTVC(val);
    fetchData(1, keySearch, fromDate, toDate, status, val, listCusSelected);
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setMaPTVC("");
    setStatus("");
    setListCusSelected([]);
    setValue("listCustomers", []);
    setListUsersSelected([]);
    setValue("listUsers", []);
    fetchData(1);
  };

  const refeshData = () => {
    fetchData(
      page,
      keySearch,
      fromDate,
      toDate,
      status,
      maPTVC,
      listCusSelected,
      listUsersSelected
    );
  };

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const handleOnChangeFilterSelect = async (values, type) => {
    if (values) {
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
        setListUsersSelected(arrUsr);
      } else {
        listUsersSelected.forEach((val) => {
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
        arrUsr
      );
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
              <h1>Quản lý vận đơn</h1>
            </div>
            {/* <div className="col-sm-6">
              <ol className="breadcrumb float-sm-right">
                <li className="breadcrumb-item">
                  <a href="#">Home</a>
                </li>
                <li className="breadcrumb-item active">Blank Page</li>
              </ol>
            </div> */}
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
                  <div className="col-sm-3">
                    <button
                      type="button"
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Tạo Vận Đơn FCL/FTL "
                      onClick={() =>
                        showModalForm(
                          SetShowModal("CreateFCL/FTL"),
                          setTitle("Tạo Mới Vận Đơn ")
                        )
                      }
                    >
                      <i className="fas fa-plus-circle">FCL/FTL</i>
                    </button>
                    <button
                      type="button"
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Tạo Vận Đơn LCL/LTL "
                      onClick={() =>
                        showModalForm(
                          SetShowModal("CreateLCL/LTL"),
                          setTitle("Tạo Mới Vận Đơn ")
                        )
                      }
                    >
                      <i className="fas fa-plus-circle">LCL/LTL</i>
                    </button>
                  </div>

                  <div className="col-sm-5">
                    <div className="row">
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
                        <div className="input-group input-group-sm">
                          <select
                            className="form-control form-control-sm"
                            onChange={(e) =>
                              handleOnChangeMaPTVC(e.target.value)
                            }
                            value={maPTVC}
                          >
                            <option value="">Tất Cả Loại Hình</option>
                            <option value="FCL">FCL</option>
                            <option value="FTL">FTL</option>
                            <option value="LCL">LCL</option>
                            <option value="LTL">LTL</option>
                          </select>
                        </div>
                      </div>
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
                  <div className="col-sm-2">
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
                      <div className="col-sm">
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
                  <div className="col-sm-2 ">
                    <div className="input-group input-group-sm">
                      <input
                        placeholder="Tìm kiếm"
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
                  title="Danh sách vận đơn"
                  direction="auto"
                  responsive
                  columns={columns}
                  data={data}
                  progressPending={loading}
                  pagination
                  paginationServer
                  paginationTotalRows={totalRows}
                  paginationRowsPerPageOptions={[10, 30, 50, 100]}
                  onChangeRowsPerPage={handlePerRowsChange}
                  onSelectedRowsChange={handleChange}
                  onChangePage={handlePageChange}
                  highlightOnHover
                  striped
                  fixedHeader
                  fixedHeaderScrollHeight="60vh"
                />
              </div>
            </div>
            <div className="card-footer"></div>
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
              style={{ maxWidth: "90%" }}
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
                    {ShowModal === "CreateLCL/LTL" && (
                      <CreateTransportLess
                        getListTransport={refeshData}
                        hideModal={hideModal}
                      />
                    )}
                    {ShowModal === "CreateFCL/FTL" && (
                      <CreateTransport
                        getListTransport={refeshData}
                        hideModal={hideModal}
                      />
                    )}
                    {ShowModal === "Edit" && (
                      <UpdateTransport
                        getListTransport={refeshData}
                        selectIdClick={selectIdClick}
                        hideModal={hideModal}
                      />
                    )}
                    {ShowModal === "EditLess" && (
                      <UpdateTransportLess
                        getListTransport={refeshData}
                        selectIdClick={selectIdClick}
                        hideModal={hideModal}
                      />
                    )}
                    {ShowModal === "ListHandling" && (
                      <HandlingPage
                        dataClick={selectIdClick}
                        hideModal={hideModal}
                      />
                    )}

                    {ShowModal === "ShowListHandling" && (
                      <HandlingByTransport
                        dataClick={selectIdClick}
                        refeshData={refeshData}
                      />
                    )}
                  </>
                </div>
              </div>
            </div>
          </div>
        </section>
      )}
    </>
  );
};

export default TransportPage;
