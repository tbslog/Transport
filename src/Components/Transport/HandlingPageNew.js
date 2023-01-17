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
import DataTable from "react-data-table-component";
import CreateTransportLess from "./CreateTransportLess";
import AddSubFeeByHandling from "./AddSubFeeByHandling";
import ApproveSubFeeByHandling from "./ApproveSubFeeByHandling";
import JoinTransports from "./JoinTransports";
import Select from "react-select";
import { ToastError } from "../Common/FuncToast";

const HandlingPageNew = () => {
  const Columns = useMemo(() => [
    {
      cell: (val) => (
        <>
          {val.statusId && (
            <>
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
                    handleEditButtonClick(val, SetShowModal("EditHandling"))
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
                  onClick={() =>
                    handleEditButtonClick(val, SetShowModal("Image"))
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
            </>
          )}
        </>
      ),
      width: "200px",
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

  const [loading, setLoading] = useState(false);
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

  const [selectedRows, setSelectedRows] = useState([]);
  const [toggledClearRows, setToggleClearRows] = useState(false);
  const [itemSelected, setItemSelected] = useState([]);

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
      fetchData(1);
    })();
  }, []);

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    status = "",
    listCusSelected = []
  ) => {
    setLoading(true);

    fromDate = !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = !toDate ? "" : moment(toDate).format("YYYY-MM-DD");

    const data = await getDataCustom(
      `BillOfLading/GetListHandlingLess?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`,
      listCusSelected
    );

    setData(data.data);
    setTotalRows(data.totalRecords);
    setLoading(false);
  };

  const refeshData = () => {
    fetchData(page, keySearch, fromDate, toDate, status, listCusSelected);
  };

  const handlePageChange = async (page) => {
    setPage(page);
    fetchData(page, keySearch, fromDate, toDate, status, listCusSelected);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const data = await getDataCustom(
      `BillOfLading/GetListHandlingLess?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`,
      listCusSelected
    );

    setData(data.data);
    setPerPage(newPerPage);
    setTotalRows(data.totalRecords);
    setLoading(false);
  };

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
            onClick={() => showConfirmDialog(val, setFuncName("Completed"))}
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

  const showConfirmDialog = (val) => {
    setSelectIdClick(val);
    setShowConfirm(true);
  };

  const handleEditButtonClick = async (value) => {
    setSelectIdClick(value);
    showModalForm();
  };

  const handleSearchClick = () => {
    fetchData(1, keySearch, fromDate, toDate, status, listCusSelected);
  };

  const handleOnChangeStatus = (val) => {
    setStatus(val);
    fetchData(1, keySearch, fromDate, toDate, val, listCusSelected);
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setStatus("");
    setListCusSelected([]);
    setValue("listCustomers", []);
    fetchData(1);
  };

  const handleClearRows = () => {
    setToggleClearRows(!toggledClearRows);
    setSelectedRows([]);
    setItemSelected([]);
  };

  const handleOnChangeCustomer = (values) => {
    if (values && values.length > 0) {
      setLoading(true);
      setValue("listCustomers", values);
      let arrCus = [];
      values.map((val) => {
        arrCus.push(val.value);
      });

      fetchData(page, keySearch, fromDate, toDate, status, arrCus);
      setLoading(false);
    } else {
      setListCusSelected([]);
      setValue("listCustomers", []);
      fetchData(page, keySearch, fromDate, toDate, status, []);
    }
  };

  const funcAgree = () => {
    if (funcName && funcName.length > 0) {
      switch (funcName) {
        case "StartRuning":
          return setRuning();
        case "CancelHandling":
          return setCancelHandling();
        case "Completed":
          return Completed();
      }
    }
  };

  const Completed = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      var update = await postData(
        `BillOfLading/SetRuning?id=${selectIdClick.maDieuPhoi}`
      );

      if (update === 1) {
        refeshData();
        setShowConfirm(false);
      } else {
        setShowConfirm(false);
      }
    }
  };

  const setRuning = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      var update = await postData(
        `BillOfLading/SetRuningLess?id=${selectIdClick.maChuyen}`
      );

      if (update === 1) {
        refeshData();
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
        `BillOfLading/CancelHandlingLess?id=${selectIdClick.maChuyen}`
      );

      if (update === 1) {
        refeshData();
        setShowConfirm(false);
      } else {
        setShowConfirm(false);
      }
    }
  };

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

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const handleOnClickMarge = () => {
    if (selectedRows && selectedRows.length > 1) {
      setItemSelected(selectedRows);
      console.log(selectedRows);
      showModalForm(SetShowModal("MargeTransport"));
    } else {
      setItemSelected([]);
      ToastError("Vui lòng chọn nhiều hơn 1 vận đơn để gộp");
      return;
    }
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
      `BillOfLading/ExportExcelHandlingLess?KeyWord=${keySearch}&fromDate=${startDate}&toDate=${endDate}&statusId=${status}`,
      listCusSelected,
      "DieuPhoi" + moment(new Date()).format("DD/MM/YYYY HHmmss")
    );
    setLoading(false);
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
              <h1>Quản Lý Điều Phối LTL/LCL</h1>
            </div>
          </div>
        </div>
      </section>
      <div className="card card-default">
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
                <button
                  type="button"
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Gộp Chuyến "
                  onClick={() => handleOnClickMarge()}
                >
                  <i className="fas fa-layer-group"></i>
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
                    itemSelected &&
                    itemSelected.length > 1 && (
                      <JoinTransports
                        getListTransport={refeshData}
                        items={itemSelected}
                        clearItems={handleClearRows}
                        hideModal={hideModal}
                      />
                    )}
                  {ShowModal === "CreateLCL/LTL" && (
                    <CreateTransportLess
                      getListTransport={refeshData}
                      hideModal={hideModal}
                    />
                  )}
                  {ShowModal === "EditHandling" && (
                    <JoinTransports
                      clearItems={handleClearRows}
                      getListTransport={refeshData}
                      selectIdClick={selectIdClick}
                      items={[]}
                      hideModal={hideModal}
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
