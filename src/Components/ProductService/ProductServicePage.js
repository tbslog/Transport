import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, getDataCustom } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import CreateProductService from "./CreateProductService";
import ApprovePriceTable from "../PriceListManage/ApprovePriceTable";

const ProductServicePage = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});
  const [listStatus, setListStatus] = useState([]);
  const [listVehicleType, setListVehicleType] = useState([]);
  const [status, setStatus] = useState();
  const [vehicleType, setVehicleType] = useState("");

  const columns = useMemo(() => [
    {
      omit: true,
      name: "Id",
      selector: (row) => row.id,
    },
    {
      name: "Mã Hợp Đồng",
      selector: (row) => row.maHopDong,
      sortable: true,
    },
    {
      name: "Tên Hợp Đồng",
      selector: (row) => row.tenHopDong,
    },
    {
      name: "Mã Cung Đường",
      selector: (row) => row.maCungDuong,
    },
    {
      name: "Tên Cung Đường",
      selector: (row) => row.tenCungDuong,
    },
    {
      name: "Phương Tiện Vận Tải",
      selector: (row) => row.maLoaiPhuongTien,
    },
    {
      name: "Phương Thức Vận Chuyển",
      selector: (row) => row.ptvc,
      omit: true,
    },
    {
      name: "Loại Hàng Hóa",
      selector: (row) => row.maLoaiHangHoa,
    },
    {
      name: "Thời gian Áp Dụng",
      selector: (row) => row.ngayApDung,
      sortable: true,
    },
    {
      name: "Thời gian Hết Hiệu Lực",
      selector: (row) => row.ngayHetHieuLuc,
      sortable: true,
    },
    ,
    {
      name: "Trạng Thái",
      selector: (row) => row.trangThai,
      sortable: true,
    },
    {
      name: "Thời gian Tạo",
      selector: (row) => moment(row.createdTime).format("DD-MM-YYYY HH:mm:ss"),
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

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    status = "",
    vehicleType = ""
  ) => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");
    const dataCus = await getData(
      `ProductService/GetListProductService?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}&vehicleType=${vehicleType}`
    );

    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(page);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `ProductService/GetListProductService?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}&vehicleType=${vehicleType}`
    );
    setPerPage(newPerPage);
    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  useEffect(() => {
    setLoading(true);
    (async () => {
      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "PriceTable",
      ]);
      setListStatus(getStatusList);
      let getListVehicleType = await getData("Common/GetListVehicleType");
      setListVehicleType(getListVehicleType);
    })();
    fetchData(1);
    setLoading(false);
  }, []);

  function formatTable(data) {
    data.map((val) => {
      val.ngayApDung = moment(val.ngayApDung).format("DD/MM/YYYY");
      val.ngayHetHieuLuc = moment(val.ngayHetHieuLuc).format("DD/MM/YYYY");
    });
    setData(data);
  }

  const handleSearchClick = () => {
    fetchData(1, keySearch, fromDate, toDate, status, vehicleType);
  };

  const handleOnChangeStatus = (value) => {
    setLoading(true);
    setStatus(value);
    fetchData(1, keySearch, fromDate, toDate, value, vehicleType);
    setLoading(false);
  };

  const getDataApprove = async () => {
    const listApprove = await getData(
      `PriceTable/GetListPriceTableApprove?PageNumber=1&PageSize=10`
    );
    return listApprove;
  };

  const handleOnChangeVehicleType = (val) => {
    setVehicleType(val);
    fetchData(1, keySearch, fromDate, toDate, status, val);
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setPerPage(10);
    fetchData(1);
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản Lý Sản Phẩm Dịch Vụ</h1>
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
                    title="Thêm mới"
                    type="button"
                    className="btn btn-sm btn-default mx-1"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("Create"),
                        setSelectIdClick({})
                      )
                    }
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                  <button
                    title="Approve List"
                    type="button"
                    className="btn btn-sm btn-default mx-1"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("ApprovePriceTable"),
                        setSelectIdClick({})
                      )
                    }
                  >
                    <i className="fas fa-check-double"></i>
                  </button>
                </div>
                <div className="col col-sm">
                  <div className="row">
                    <div className="col col-sm">
                      <div className="input-group input-group-sm">
                        <select
                          className="form-control form-control-sm"
                          onChange={(e) =>
                            handleOnChangeVehicleType(e.target.value)
                          }
                          value={vehicleType}
                        >
                          <option value="">Loại Phương Tiện</option>
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
                      </div>
                    </div>
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
                title="Danh sách Sản Phẩm Dịch Vụ"
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
          <div className="card-footer">
            <div className="row"></div>
          </div>
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
                  {ShowModal === "Create" && (
                    <CreateProductService
                      getListPriceTable={fetchData}
                      selectIdClick={selectIdClick}
                      listStatus={listStatus}
                    />
                  )}
                  {ShowModal === "ApprovePriceTable" && (
                    <ApprovePriceTable
                      getDataApprove={getDataApprove}
                      checkShowModal={modal}
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

export default ProductServicePage;
