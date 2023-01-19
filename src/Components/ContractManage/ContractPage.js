import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import { getData, postFile, getDataCustom } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import AddContract from "./AddContract";
import EditContract from "./EditContract";
import DatePicker from "react-datepicker";
import AddPriceTable from "../PriceListManage/AddPriceTable";

const ContractPage = () => {
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

  const [tabIndex, setTabIndex] = useState(0);
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const [priceTable, setPriceTable] = useState([]);
  const [listStatus, setListStatus] = useState([]);
  const [listContractType, setListContractType] = useState([]);
  const [contractType, setContractType] = useState("");
  const [custommerType, setCustommerType] = useState("");

  const columns = useMemo(() => [
    {
      name: "Cập nhật",
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("Edit"))}
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Cập Nhật Thông Tin"
        >
          <i className="far fa-edit"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    // {
    //   name: "Chi Tiết",
    //   cell: (val) => (
    //     <button
    //       onClick={() => handleOnclickDetail()}
    //       type="button"
    //       className="btn btn-sm btn-default"
    //     >
    //       <i className="fas fa-file-contract"></i>
    //     </button>
    //   ),
    //   ignoreRowClick: true,
    //   allowOverflow: true,
    //   button: true,
    // },
    {
      name: "Bảng Giá",
      cell: (val) => (
        <button
          onClick={() =>
            handleOnclickPriceTable(val, SetShowModal("PriceTable"))
          }
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Xem Bảng Giá"
        >
          <i className="fas fa-money-check-alt"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "Mã Hợp Đồng",
      selector: (row) => row.maHopDong,
    },
    {
      name: "Tên Hợp Đồng",
      selector: (row) => row.tenHienThi,
    },
    {
      name: "Phân Loại",
      selector: (row) => row.soHopDongCha,
      sortable: true,
    },
    // {
    //   name: "Phân Loại",
    //   selector: (row) => row.phanLoaiHopDong,
    //   sortable: true,
    // },
    {
      name: "Mã Khách Hàng",
      selector: (row) => row.maKh,
    },
    {
      name: "Tên Khách Hàng",
      selector: (row) => row.tenKH,
    },
    {
      name: "Trạng thái",
      selector: (row) => row.trangThai,
    },
    {
      name: "Thời Gian Bắt Đầu",
      selector: (row) => row.thoiGianBatDau,
      sortable: true,
    },
    {
      name: "Thời Gian Kết Thúc",
      selector: (row) => row.thoiGianKetThuc,
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

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const handleEditButtonClick = async (val) => {
    showModalForm();
    const getDataContract = await getData(
      `Contract/GetContractById?Id=${val.maHopDong}`
    );

    setSelectIdClick(getDataContract);
  };

  const handleOnclickPriceTable = async (val) => {
    setPriceTable(val);
    showModalForm();
  };

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    contractType = "",
    customerType = ""
  ) => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }

    const dataCus = await getData(
      `Contract/GetListContract?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&contractType=${contractType}&customerType=${customerType}`
    );
    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    fetchData(
      1,
      keySearch,
      !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD"),
      !toDate ? "" : moment(toDate).format("YYYY-MM-DD"),
      "",
      tabIndex === 0 ? "KH" : "NCC"
    );
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `Contract/GetListContract?PageNumber=${page}&PageSize=${perPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&contractType=${contractType}&customerType=${
        tabIndex === 0 ? "KH" : "NCC"
      }`
    );

    formatTable(dataCus.data);
    setPerPage(newPerPage);
    setLoading(false);
  };

  const hideModal = () => {
    modal.hide();
  };

  useEffect(() => {
    setLoading(true);

    (async () => {
      const getListContractType = await getData(`Common/GetListContractType`);
      setListContractType(getListContractType);

      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "Contract",
      ]);
      setListStatus(getStatusList);
    })();

    setLoading(false);
  }, []);

  function formatTable(data) {
    data.map((val) => {
      val.thoiGianBatDau = moment(val.thoiGianBatDau).format(" DD/MM/YYYY");
      val.thoiGianKetThuc = moment(val.thoiGianKetThuc).format(" DD/MM/YYYY");

      val.maBangGia = val.maBangGia === null ? "Empty" : val.maBangGia;
    });
    setData(data);
  }

  const handleSearchClick = () => {
    fetchData(
      1,
      keySearch,
      !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD"),
      !toDate ? "" : moment(toDate).format("YYYY-MM-DD"),
      "",
      tabIndex === 0 ? "KH" : "NCC"
    );
  };

  const handleExcelImportClick = (e) => {
    setLoading(true);
    var file = e.target.files[0];
    e.target.value = null;

    const importExcelCus = postFile("Contract/ReadFileExcel", {
      formFile: file,
    });
    setLoading(false);
  };

  const handleRefeshDataClick = () => {
    fetchData(1, "", "", "", "", "KH");
    setContractType("");
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setTabIndex(0);
  };

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex);
    let customerType = tabIndex === 0 ? "KH" : "NCC";
    fetchData(1, "", "", "", "", customerType);
    setCustommerType(customerType);
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản lý hợp đồng</h1>
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
                    gloss="Tạo Mới Hợp Hồng"
                    onClick={() => showModalForm(SetShowModal("Create"))}
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                </div>
                <div className="col-sm-3">
                  <div className="row">
                    <div className="col col-sm"></div>
                    <div className="col col-sm">
                      {/* <div className="input-group input-group-sm">
                        <select
                          className="form-control form-control-sm"
                          onChange={(e) =>
                            handleOnChangeContractType(e.target.value)
                          }
                          value={contractType}
                        >
                          <option value="">Phân loại HĐ</option>
                          {listContractType &&
                            listContractType.map((val) => {
                              return (
                                <option
                                  value={val.maLoaiHopDong}
                                  key={val.maLoaiHopDong}
                                >
                                  {val.tenLoaiHopDong}
                                </option>
                              );
                            })}
                        </select>
                      </div> */}
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
            <Tabs
              selectedIndex={tabIndex}
              onSelect={(index) => HandleOnChangeTabs(index)}
            >
              <TabList>
                <Tab>Hợp Đồng Khách Hàng</Tab>
                <Tab>Hợp Đồng Nhà Cung Cấp</Tab>
              </TabList>

              <TabPanel>
                <div className="container-datatable" style={{ height: "50vm" }}>
                  <DataTable
                    title="Danh sách hợp đồng khách hàng"
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
                    striped
                    direction="auto"
                    responsive
                  />
                </div>
              </TabPanel>
              <TabPanel>
                <div className="container-datatable" style={{ height: "50vm" }}>
                  <DataTable
                    title="Danh sách hợp đồng nhà cung cấp"
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
                    striped
                    direction="auto"
                    responsive
                  />
                </div>
              </TabPanel>
            </Tabs>
          </div>
          <div className="card-footer">
            {/* <div className="row">
              <div className="col-sm-3">
                <a
                  href=""
                  download="Template Thêm mới cung đường.xlsx"
                  className="btn btn-sm btn-default mx-1"
                >
                  <i className="fas fa-download"></i>
                </a>
                <div className="upload-btn-wrapper">
                  <button className="btn btn-sm btn-default mx-1">
                    <i className="fas fa-upload"></i>
                  </button>
                  <input
                    type="file"
                    name="myfile"
                    onChange={(e) => handleExcelImportClick(e)}
                  />
                </div>
              </div>
            </div> */}
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
            style={{ maxWidth: "88%" }}
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
                    <EditContract
                      selectIdClick={selectIdClick}
                      getListContract={fetchData}
                      listContractType={listContractType}
                      listStatus={listStatus}
                      hideModal={hideModal}
                      tabIndex={tabIndex}
                    />
                  )}
                  {ShowModal === "Create" && (
                    <AddContract
                      getListContract={fetchData}
                      listContractType={listContractType}
                      listStatus={listStatus}
                      hideModal={hideModal}
                      tabIndex={tabIndex}
                    />
                  )}
                  {ShowModal === "PriceTable" && (
                    <AddPriceTable
                      getListPriceTable={fetchData}
                      selectIdClick={priceTable}
                      listStatus={listStatus}
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

export default ContractPage;
