import { useMemo, useState, useEffect, useRef } from "react";
import { getData, getDataCustom } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import AddPriceTable from "./AddPriceTable";
import ApprovePriceTable from "./ApprovePriceTable";

const customStyles = {
  rows: {
    style: {
      backgroundColor: "#EFE5D0",
    },
  },
  headCells: {
    style: {
      backgroundColor: "#EFE5D0",
    },
  },
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

const PriceTablePage = () => {
  const [data, setData] = useState([]);

  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [page, setPage] = useState(0);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [tabIndex, setTabIndex] = useState(0);
  const [custommerType, setCustommerType] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const [selectIdClick, setSelectIdClick] = useState({});

  const [title, setTitle] = useState("");

  const paginationComponentOptions = {
    rowsPerPageText: "Dữ liệu mỗi trang",
    rangeSeparatorText: "của",
    selectAllRowsItem: true,
    selectAllRowsItemText: "Tất cả",
  };

  const columns = useMemo(() => [
    {
      name: <div>Chuỗi</div>,
      selector: (row) => row.tenChuoi,
      sortable: true,
    },
    {
      name: <div>Mã Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.maKH}</div>,
    },
    {
      name: <div>Tên Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.tenKH}</div>,
    },
    {
      name: <div>Mã Số Thuế</div>,
      selector: (row) => <div className="text-wrap">{row.maSoThue}</div>,
    },

    {
      name: <div>Số Điện Thoại</div>,
      selector: (row) => <div className="text-warp">{row.soDienThoai}</div>,
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

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    custommerType
  ) => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");

    const dataCus = await getDataCustom(
      `PriceTable/GetListPriceTable?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&customerType=${custommerType}`
    );

    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    setPage(page);
    fetchData(page, keySearch, fromDate, toDate, custommerType);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getDataCustom(
      `PriceTable/GetListPriceTable?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&customerType=${custommerType}`
    );
    setPerPage(newPerPage);
    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  useEffect(() => {
    setLoading(true);
    fetchData(1, "", "", "", "KH");
    setCustommerType("KH");
    setLoading(false);
  }, []);

  const handleSearchClick = () => {
    fetchData(page, keySearch, fromDate, toDate, custommerType);
  };

  const ReloadData = () => {
    fetchData(page, keySearch, fromDate, toDate, custommerType);
  };

  const handleRefeshDataClick = () => {
    setPerPage(10);
    setKeySearch("");
    setFromDate("");
    setToDate("");
    fetchData(1, "", "", "", custommerType);
  };

  const getDataApprove = async () => {
    const listApprove = await getData(
      `PriceTable/GetListPriceTableApprove?PageNumber=1&PageSize=10`
    );
    return listApprove;
  };

  const ExpandedComponent = ({ data }) => {
    if (
      data.listContractOfCustomers &&
      data.listContractOfCustomers.length > 0
    ) {
      return (
        <div className="container-datatable" style={{ height: "50vm" }}>
          <DataTable
            columns={[
              {
                cell: (val) => (
                  <>
                    <button
                      onClick={() =>
                        showModalForm(
                          setSelectIdClick(val),
                          SetShowModal("PriceTable"),
                          setTitle("Thông Tin Bảng Giá")
                        )
                      }
                      type="button"
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Xem Bảng Giá"
                    >
                      <i className="fas fa-money-check-alt"></i>
                    </button>
                  </>
                ),
                width: "200px",
                ignoreRowClick: true,
                allowOverflow: true,
                button: true,
              },
              {
                selector: (row) => row.maKH,
                omit: true,
              },
              {
                name: "Mã Hợp Đồng",
                selector: (row) => row.maHopDong,
              },
              {
                name: "Tên Hợp Đồng",
                selector: (row) => row.tenHopDong,
              },
              {
                name: "Loại Hình Hợp Tác",
                selector: (row) => row.loaiHinhHopTac,
              },
              {
                name: "Sản Phẩm Dịch Vụ",
                selector: (row) => row.sanPhamDichVu,
              },
            ]}
            data={data.listContractOfCustomers}
            progressPending={loading}
            highlightOnHover
            direction="auto"
            customStyles={customStyles}
            responsive
          />
        </div>
      );
    }
  };

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex);
    let customerType = tabIndex === 0 ? "KH" : "NCC";
    fetchData(1, "", "", "", customerType);
    setCustommerType(customerType);
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản lý bảng giá</h1>
            </div>
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
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Thêm Mới Bảng Giá"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("Create"),
                        setSelectIdClick({}),
                        setTitle("Tạo Mới Bảng Giá")
                      )
                    }
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Duyệt Bảng Giá"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("ApprovePriceTable"),
                        setSelectIdClick({}),
                        setTitle("Duyệt Bảng Giá")
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
              <div className="row">
                <div className="col col-6"></div>
              </div>
            </div>
          </div>
          <div className="card-body">
            <Tabs
              selectedIndex={tabIndex}
              onSelect={(index) => HandleOnChangeTabs(index)}
            >
              <TabList>
                <Tab>Bảng Giá Khách Hàng</Tab>
                <Tab>Bảng Giá Nhà Cung Cấp</Tab>
              </TabList>

              <TabPanel>
                <div className="container-datatable" style={{ height: "50vm" }}>
                  <DataTable
                    columns={columns}
                    data={data}
                    progressPending={loading}
                    pagination
                    paginationServer
                    paginationTotalRows={totalRows}
                    paginationRowsPerPageOptions={[10, 30, 50, 100]}
                    onChangeRowsPerPage={handlePerRowsChange}
                    onChangePage={handlePageChange}
                    expandableRows
                    expandableRowsComponent={ExpandedComponent}
                    highlightOnHover
                    striped
                    direction="auto"
                    responsive
                    fixedHeader
                    fixedHeaderScrollHeight="60vh"
                  />
                </div>
              </TabPanel>
              <TabPanel>
                <div className="container-datatable" style={{ height: "50vm" }}>
                  <DataTable
                    columns={columns}
                    data={data}
                    progressPending={loading}
                    pagination
                    paginationServer
                    paginationTotalRows={totalRows}
                    paginationRowsPerPageOptions={[10, 30, 50, 100]}
                    onChangeRowsPerPage={handlePerRowsChange}
                    onChangePage={handlePageChange}
                    expandableRows
                    expandableRowsComponent={ExpandedComponent}
                    highlightOnHover
                    striped
                    direction="auto"
                    responsive
                    fixedHeader
                    fixedHeaderScrollHeight="60vh"
                  />
                </div>
              </TabPanel>
            </Tabs>
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
                  {ShowModal === "Create" && (
                    <AddPriceTable selectIdClick={selectIdClick} />
                  )}
                  {ShowModal === "PriceTable" && (
                    <AddPriceTable
                      getListPriceTable={fetchData}
                      selectIdClick={selectIdClick}
                    />
                  )}
                  {ShowModal === "ApprovePriceTable" && (
                    <ApprovePriceTable
                      reLoadData={ReloadData}
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
