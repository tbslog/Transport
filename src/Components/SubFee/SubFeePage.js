import { useMemo, useState, useEffect, useRef } from "react";
import { getData } from "../Common/FuncAxios";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import CreateSubFee from "./CreateSubFee";
import ApproveSubFee from "./ApproveSubFee";
import SubfeeDetail from "./SubfeeDetail";

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

const SubFeePage = () => {
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
  const [custommerType, setCustommerType] = useState("");
  const [selectIdClick, setSelectIdClick] = useState({});
  const [tabIndex, setTabIndex] = useState(0);

  const [title, setTitle] = useState("");

  const columns = useMemo(() => [
    {
      name: <div>Chuỗi</div>,
      selector: (row) => row.tenChuoi,
      sortable: true,
    },
    {
      name: <div>Mã Đối Tác</div>,
      selector: (row) => <div className="text-wrap">{row.maKH}</div>,
    },
    {
      name: <div>Tên Đối Tác</div>,
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
    const dataCus = await getData(
      `SubFeePrice/GetListSubFeePrice?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&customerType=${custommerType}`
    );

    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(page, keySearch, fromDate, toDate, custommerType);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `SubFeePrice/GetListSubFeePrice?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&customerType=${custommerType}`
    );
    setPerPage(newPerPage);
    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  useEffect(() => {
    setLoading(true);

    fetchData(1, "", "", "", custommerType);
    setLoading(false);
  }, []);

  function formatTable(data) {
    data.map((val) => {
      !val.approvedDate
        ? (val.approvedDate = "")
        : (val.approvedDate = moment(val.approvedDate).format("DD/MM/YYYY"));
      !val.deactiveDate
        ? (val.deactiveDate = "")
        : (val.deactiveDate = moment(val.deactiveDate).format("DD/MM/YYYY"));
    });
    setData(data);
  }

  const handleSearchClick = () => {
    fetchData(1, keySearch, fromDate, toDate, custommerType);
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setPerPage(10);
    fetchData(1, "", "", "", custommerType);
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
                          SetShowModal("SubfeePrice"),
                          setTitle("Thông Tin Phụ Phí")
                        )
                      }
                      type="button"
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Xem Phụ Phí"
                    >
                      <i className="fas fa-hand-holding-usd"></i>
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
              <h1>Quản Lý Phụ Phí Theo Hợp Đồng</h1>
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
                    gloss="Thêm Mới Phụ Phí"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("Create"),
                        setSelectIdClick({}),
                        setTitle("Tạo Mới Phụ Phí Theo Hợp Đồng")
                      )
                    }
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Duyệt Phụ Phí"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("ApproveSubFee"),
                        setSelectIdClick({}),
                        setTitle("Duyệt Phụ Phí Theo Hợp Đồng")
                      )
                    }
                  >
                    <i className="fas fa-check-double"></i>
                  </button>
                </div>
                <div className="col col-sm">
                  <div className="row">
                    <div className="col col-sm"></div>
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
            <Tabs
              selectedIndex={tabIndex}
              onSelect={(index) => HandleOnChangeTabs(index)}
            >
              <TabList>
                <Tab>Phụ Phí Khách Hàng</Tab>
                <Tab>Phụ Phí Nhà Cung Cấp</Tab>
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
                    dense
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
                    dense
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
                    <CreateSubFee getListSubFee={fetchData} />
                  )}
                  {ShowModal === "ApproveSubFee" && (
                    <ApproveSubFee
                      getListSubFee={fetchData}
                      checkShowModal={modal}
                    />
                  )}
                  {ShowModal === "SubfeePrice" && (
                    <SubfeeDetail selectIdClick={selectIdClick} title={title} />
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

export default SubFeePage;
