import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, getDataCustom } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import CreateTransport from "./CreateTransport";
import UpdateTransport from "./UpdateTransport";
import HandlingPage from "./HandlingPage";
import CreateTransportLess from "./CreateTransportLess";

const TransportPage = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [listStatus, setListStatus] = useState([]);
  const [status, setStatus] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("Edit"))}
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Chỉnh Sửa"
        >
          <i className="far fa-edit"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    // {
    //   cell: (val) => (
    //     <button
    //       onClick={() =>
    //         handleEditButtonClick(
    //           val,
    //           val.maTrangThai === 8
    //             ? SetShowModal("Handling")
    //             : SetShowModal("ListHandling")
    //         )
    //       }
    //       type="button"
    //       className="btn btn-sm btn-default"
    //     >
    //       <i className="fas fa-random"></i>
    //     </button>
    //   ),
    //   ignoreRowClick: true,
    //   allowOverflow: true,
    //   button: true,
    // },
    {
      selector: (row) => <div className="text-wrap">{row.maVanDon}</div>,
      omit: true,
    },
    {
      name: <div>Mã Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.maVanDonKH}</div>,
    },
    {
      name: <div>Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.tenKH}</div>,
    },
    {
      name: <div>PTVC</div>,
      selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
    },
    {
      name: <div>Loại Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.loaiVanDon}</div>,
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
    // {
    //   name: "Điểm Lấy Rỗng",
    //   selector: (row) => row.diemLayRong,
    //   width: "auto",
    // },
    // {
    //   name: <div>Điểm Lấy Hàng</div>,
    //   selector: (row) => <div className="text-wrap">{row.diemLayHang}</div>,
    // },
    // {
    //   name: <div>Tổng Trọng Lượng</div>,
    //   selector: (row) => row.diemTraHang,
    // },
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
    // {
    //   name: <div>Thời Gian Có Mặt</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {moment(row.thoiGianCoMat).format("DD/MM/YYYY HH:mm")}
    //     </div>
    //   ),
    //   sortable: true,
    // },
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
    // {
    //   name: <div>Thời Gian Lấy Hàng</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {moment(row.thoiGianLayHang).format("DD/MM/YYYY HH:mm")}
    //     </div>
    //   ),
    //   sortable: true,
    // },
    // {
    //   name: <div>Thời Gian Trả Hàng</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {moment(row.thoiGianTraHang).format("DD/MM/YYYY HH:mm")}
    //     </div>
    //   ),
    //   sortable: true,
    // },
    {
      selector: (row) => row.maTrangThai,
      sortable: true,
      omit: true,
    },
    {
      name: <div>Trạng Thái</div>,
      selector: (row) => <div className="text-wrap">{row.trangThai}</div>,
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
    status = ""
  ) => {
    setLoading(true);
    const datatransport = await getData(
      `BillOfLading/GetListTransport?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&StatusId=${status}&fromDate=${fromDate}&toDate=${toDate}&transportType=FULL`
    );

    setData(datatransport.data);
    setTotalRows(datatransport.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    fetchData(
      page,
      keySearch,
      !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD"),
      !toDate ? "" : moment(toDate).format("YYYY-MM-DD"),
      status
    );
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const datatransport = await getData(
      `BillOfLading/GetListTransport?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&StatusId=${status}&fromDate=${fromDate}&toDate=${toDate}&transportType=FULL`
    );
    setData(datatransport.data);
    setPerPage(newPerPage);
    setLoading(false);
  };

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const handleEditButtonClick = async (value) => {
    let getTransportById = await getData(
      `BillOfLading/GetTransportById?transportId=${value.maVanDon}`
    );

    setSelectIdClick(getTransportById);
    showModalForm();
  };

  const handleSearchClick = () => {
    fetchData(
      1,
      keySearch,
      !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD"),
      !toDate ? "" : moment(toDate).format("YYYY-MM-DD"),
      status
    );
  };

  const handleOnChangeStatus = (val) => {
    setStatus(val);
    fetchData(1, keySearch, fromDate, toDate, val);
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    fetchData(1);
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
                    onClick={() => showModalForm(SetShowModal("CreateFCL/FTL"))}
                  >
                    <i className="fas fa-plus-circle">FCL/FTL</i>
                  </button>
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Tạo Vận Đơn LCL/LTL "
                    onClick={() => showModalForm(SetShowModal("CreateLCL/LTL"))}
                  >
                    <i className="fas fa-plus-circle">LCL/LTL</i>
                  </button>
                </div>

                <div className="col-sm-3">
                  <div className="row">
                    <div className="col col-sm"></div>
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
                <div className="col-sm-3">
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
                <div className="col-sm-3 ">
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
                onSelectedRowsChange={handleChange}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
                striped
              />
            </div>
          </div>
          <div className="card-footer"></div>
        </div>
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
                    <CreateTransportLess getListTransport={fetchData} />
                  )}
                  {ShowModal === "CreateFCL/FTL" && (
                    <CreateTransport getListTransport={fetchData} />
                  )}
                  {ShowModal === "Edit" && (
                    <UpdateTransport
                      getListTransport={fetchData}
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
                </>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
};

export default TransportPage;
