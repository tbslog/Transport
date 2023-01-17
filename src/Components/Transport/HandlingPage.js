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
import HandlingImage from "./HandlingImage";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import AddSubFeeByHandling from "./AddSubFeeByHandling";
import ApproveSubFeeByHandling from "./ApproveSubFeeByHandling";
import { ToastError } from "../Common/FuncToast";

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
  const [loading, setLoading] = useState(false);

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
              onClick={() => handleEditButtonClick(val, SetShowModal("Edit"))}
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
                handleEditButtonClick(val, SetShowModal("addSubFee"))
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
              onClick={() => handleEditButtonClick(val, SetShowModal("Image"))}
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
      selector: (row) => row.maDieuPhoi,
      omit: true,
    },
    {
      selector: (row) => row.maVanDon,
      omit: true,
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
    {
      name: <div>Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.maKH}</div>,
      sortable: true,
    },
    {
      name: <div>Đơn Vị Vận Tải</div>,
      selector: (row) => <div className="text-wrap">{row.donViVanTai}</div>,
      sortable: true,
    },
    {
      name: "PTVC",
      selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
      sortable: true,
    },
    // {
    //   name: <div>Cung Đường</div>,
    //   selector: (row) => <div className="text-wrap">{row.cungDuong}</div>,
    //   sortable: true,
    // },
    {
      name: <div>Điểm Lấy Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.diemLayHang}</div>,
      sortable: true,
    },
    {
      name: <div>Điểm Trả Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.diemTraHang}</div>,
      sortable: true,
    },
    {
      name: <div>Điểm Lấy Rỗng</div>,
      selector: (row) => <div className="text-wrap">{row.diemLayRong}</div>,
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
    // {
    //   name: <div>Số Kiện</div>,
    //   selector: (row) => row.soKien,
    //   sortable: true,
    // },
    {
      name: <div>Trạng Thái</div>,
      selector: (row) => <div className="text-wrap">{row.trangThai}</div>,
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
    setLoading(true);
    (async () => {
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
    setLoading(false);
  }, []);

  const renderButton = (val) => {
    switch (val.statusId) {
      case 27:
        return val.ptVanChuyen.includes("CONT") ? (
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
    listCusSelected = []
  ) => {
    setLoading(true);
    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");
    const dataCus = await getDataCustom(
      `BillOfLading/GetListHandling?transportId=${transportId}&PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`,
      listCusSelected
    );

    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    setPage(page);
    fetchData(
      transportId,
      page,
      keySearch,
      fromDate,
      toDate,
      status,
      listCusSelected
    );
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getDataCustom(
      `BillOfLading/GetListHandling?transportId=${transportId}&PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`,
      listCusSelected
    );
    setPerPage(newPerPage);
    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
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
        fetchData(
          transportId,
          page,
          keySearch,
          fromDate,
          toDate,
          status,
          listCusSelected
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
        fetchData(
          transportId,
          page,
          keySearch,
          fromDate,
          toDate,
          status,
          listCusSelected
        );
        setShowConfirm(false);
      } else {
        setShowConfirm(false);
      }
    }
  };

  const funcAgree = () => {
    if (funcName && funcName.length > 0) {
      switch (funcName) {
        case "StartRuning":
          return setRuning();
        case "CancelHandling":
          return setCancelHandling();
        case "CloneHandling":
          return CloneHandling();
        case "RemoveHandling":
          return RemoveHandling();
      }
    }
  };

  const CloneHandling = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      var update = await postData(
        `BillOfLading/CloneHandling?id=${selectIdClick.maDieuPhoi}`
      );

      if (update === 1) {
        fetchData(
          transportId,
          page,
          keySearch,
          fromDate,
          toDate,
          status,
          listCusSelected
        );
        setShowConfirm(false);
      } else {
        setShowConfirm(false);
      }
    }
  };

  const RemoveHandling = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      var update = await postData(
        `BillOfLading/RemoveHandling?id=${selectIdClick.maDieuPhoi}`
      );

      if (update === 1) {
        fetchData(
          transportId,
          page,
          keySearch,
          fromDate,
          toDate,
          status,
          listCusSelected
        );
        setShowConfirm(false);
      } else {
        setShowConfirm(false);
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
      listCusSelected
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
      listCusSelected
    );
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setStatus("");
    setListCusSelected([]);
    setValue("listCustomers", []);
    setPerPage(10);
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
      listCusSelected
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

    setLoading(true);

    let startDate = moment(fromDate).format("YYYY-MM-DD");
    let endDate = moment(toDate).format("YYYY-MM-DD");

    const getFileDownLoad = await getFilePost(
      `BillOfLading/ExportExcelHandLing?KeyWord=${keySearch}&fromDate=${startDate}&toDate=${endDate}&statusId=${status}`,
      listCusSelected,
      "DieuPhoi" + moment(new Date()).format("DD/MM/YYYY HHmmss")
    );
    setLoading(false);
  };

  const handleOnChangeCustomer = async (values) => {
    if (values && values.length > 0) {
      setLoading(true);
      setValue("listCustomers", values);
      let arrCus = [];
      values.map((val) => {
        arrCus.push(val.value);
      });

      fetchData(transportId, page, keySearch, fromDate, toDate, status, arrCus);
      setLoading(false);
    } else {
      setListCusSelected([]);
      setValue("listCustomers", []);
      fetchData(transportId, page, keySearch, fromDate, toDate, status, []);
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
                        setSelectIdClick({})
                      )
                    }
                  >
                    <i className="fas fa-check-double"></i>
                  </button>
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
                          onChange={(field) => handleOnChangeCustomer(field)}
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
                          onChange={(e) => handleOnChangeStatus(e.target.value)}
                          value={status}
                        >
                          <option value="">Tất Cả Trạng Thái</option>
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
                responsive
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
    </>
  );
};

export default HandlingPage;
