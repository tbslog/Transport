import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, getDataCustom } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import AddPriceTable from "./AddPriceTable";
import ApprovePriceTable from "./ApprovePriceTable";

const PriceTablePage = () => {
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
  const [vehicleType, setVehicleType] = useState("");
  const [listGoodsType, setListGoodsType] = useState([]);
  const [goodsType, setGoodsType] = useState("");

  const paginationComponentOptions = {
    rowsPerPageText: "Dữ liệu mỗi trang",
    rangeSeparatorText: "của",
    selectAllRowsItem: true,
    selectAllRowsItemText: "Tất cả",
  };

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          title="Cập nhật"
          onClick={() => handleEditButtonClick(val, SetShowModal("Create"))}
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
      name: "Mã Hợp Đồng",
      selector: (row) => row.maHopDong,
      sortable: true,
    },
    {
      name: "Tên Hợp Đồng",
      selector: (row) => row.tenHopDong,
    },
    {
      name: "Tên khách hàng",
      selector: (row) => row.tenKH,
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
      name: "Thời gian Áp Dụng",
      selector: (row) => row.ngayApDung,
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

  const handleEditButtonClick = async (val) => {
    showModalForm();
    setSelectIdClick(val);
  };

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    vehicleType = "",
    goodsType = ""
  ) => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");
    const dataCus = await getData(
      `PriceTable/GetListPriceTable?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&goodsType=${goodsType}&vehicleType=${vehicleType}`
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
      `PriceTable/GetListPriceTable?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&goodsType=${goodsType}&vehicleType=${vehicleType}`
    );
    setPerPage(newPerPage);
    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);

    setLoading(false);
  };

  useEffect(() => {
    setLoading(true);

    (async () => {
      let getListCustommerGroup = await getData(`Common/GetListVehicleType`);
      setListVehicleType(getListCustommerGroup);

      let getListCustommerType = await getData(`Common/GetListGoodsType`);
      setListGoodsType(getListCustommerType);

      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "common",
      ]);
      setListStatus(getStatusList);
    })();

    fetchData(1);
    setLoading(false);
  }, []);

  function formatTable(data) {
    data.map((val) => {
      val.ngayApDung = moment(val.ngayApDung).format("DD/MM/YYYY");
    });
    setData(data);
  }

  const handleOnChangeVehicleType = (value) => {
    setLoading(true);
    setVehicleType(value);
    fetchData(1, keySearch, fromDate, toDate, value, goodsType);
    setLoading(false);
  };

  const handleOnChangeGoodsType = (value) => {
    setLoading(true);
    setGoodsType(value);
    fetchData(1, keySearch, fromDate, toDate, vehicleType, value);
    setLoading(false);
  };

  const handleSearchClick = () => {
    fetchData(1, keySearch, fromDate, toDate, vehicleType, goodsType);
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setPerPage(10);
    fetchData(1);
  };

  const getDataApprove = async () => {
    const listApprove = await getData(
      `PriceTable/GetListPriceTableApprove?PageNumber=1&PageSize=10`
    );
    return listApprove;
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản lý bảng giá</h1>
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
                          <option value="">Phương tiện vận tải</option>
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
                          onChange={(e) =>
                            handleOnChangeGoodsType(e.target.value)
                          }
                          value={goodsType}
                        >
                          <option value="">Loại Hàng Hóa</option>
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
                title="Danh sách bảng giá"
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                paginationTotalRows={totalRows}
                paginationComponentOptions={paginationComponentOptions}
                selectableRows
                onSelectedRowsChange={handleChange}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
              />
            </div>
          </div>
          <div className="card-footer">
            <div className="row">
              {/* <div className="col-sm-3">
                <a
                  title="Tải Template Excel"
                  href={FileExcelImport}
                  download="Template Thêm mới Khách hàng.xlsx"
                  className="btn btn-sm btn-default mx-1"
                >
                  <i className="fas fa-download"></i>
                </a>
                <div className="upload-btn-wrapper">
                  <button
                    className="btn btn-sm btn-default mx-1"
                    title="Upload file Excel"
                  >
                    <i className="fas fa-upload"></i>
                  </button>
                  <input
                    type="file"
                    name="myfile"
                    onChange={(e) => handleExcelImportClick(e)}
                  />
                </div>
              </div> */}
            </div>
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
                    <AddPriceTable
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

export default PriceTablePage;
