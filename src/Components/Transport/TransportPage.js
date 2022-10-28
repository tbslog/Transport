import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postFile, getDataCustom } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import CreateTransport from "./CreateTransport";
import UpdateTransport from "./UpdateTransport";
import CreateHandling from "./CreateHandling";
import ListHandling from "./ListHandling";

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

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("Edit"))}
          type="button"
          className="btn btn-sm btn-default"
        >
          <i className="far fa-edit"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      cell: (val) => (
        <button
          onClick={() =>
            handleEditButtonClick(
              val,
              val.maTrangThai === 8
                ? SetShowModal("Handling")
                : SetShowModal("ListHandling")
            )
          }
          type="button"
          className="btn btn-sm btn-default"
        >
          <i className="fas fa-random"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "Mã Vận Đơn",
      selector: (row) => row.maVanDon,
    },
    {
      name: "Loại Vận Đơn",
      selector: (row) => row.loaiVanDon,
    },
    {
      name: "Mã Cung Đường",
      selector: (row) => row.maCungDuong,
      sortable: true,
      width: "auto",
      omit: true,
    },
    {
      name: "Tên Cung Đường",
      selector: (row) => row.tenCungDuong,
      sortable: true,
      width: "auto",
    },
    // {
    //   name: "Điểm Lấy Rỗng",
    //   selector: (row) => row.diemLayRong,
    //   width: "auto",
    // },
    {
      name: "Điểm Lấy Hàng",
      selector: (row) => row.diemLayHang,
    },
    {
      name: "Điểm Trả Hàng",
      selector: (row) => row.diemTraHang,
    },
    {
      name: "Tổng Trọng Lượng",
      selector: (row) => row.tongKhoiLuong,
      sortable: true,
    },
    {
      name: "Tổng Thể Tích",
      selector: (row) => row.tongTheTich,
      sortable: true,
    },
    {
      name: "Thời Gian Lấy Hàng",
      selector: (row) => moment(row.thoiGianLayHang).format("DD/MM/YYYY HH:mm"),
      sortable: true,
    },
    {
      name: "Thời Gian Trả Hàng",
      selector: (row) => moment(row.thoiGianTraHang).format("DD/MM/YYYY HH:mm"),
      sortable: true,
    },
    {
      selector: (row) => row.maTrangThai,
      sortable: true,
      omit: true,
    },
    {
      name: "Trạng Thái",
      selector: (row) => row.trangThai,
      sortable: true,
    },
    {
      name: "Thời Gian Lập Đơn",
      selector: (row) =>
        moment(row.thoiGianTaoDon).format("DD/MM/YYYY HH:mm:ss"),
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
      `BillOfLading/GetListTransport?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&StatusId=${status}&fromDate=${fromDate}&toDate=${toDate}`
    );

    setData(datatransport.data);
    setTotalRows(datatransport.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(page);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const datatransport = await getData(
      `BillOfLading/GetListTransport?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}`
    );
    setData(datatransport);
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
      !toDate ? "" : moment(toDate).format("YYYY-MM-DD")
    );
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
                    className="btn btn-sm btn-default mx-1"
                    onClick={() => showModalForm(SetShowModal("Create"))}
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                </div>
                <div className="col-sm-3">
                  <div className="row">
                    <div className="col col-sm"></div>
                    <div className="col col-sm"></div>
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
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                paginationTotalRows={totalRows}
                onSelectedRowsChange={handleChange}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
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
                  {ShowModal === "Create" && (
                    <CreateTransport getListTransport={fetchData} />
                  )}
                  {ShowModal === "Edit" && (
                    <UpdateTransport
                      getListTransport={fetchData}
                      selectIdClick={selectIdClick}
                      hideModal={hideModal}
                    />
                  )}
                  {ShowModal === "Handling" && (
                    <CreateHandling
                      selectIdClick={selectIdClick}
                      reOpenModal={handleEditButtonClick}
                      hideModal={hideModal}
                      getListTransport={fetchData}
                    />
                  )}
                  {ShowModal === "ListHandling" && (
                    <ListHandling
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
