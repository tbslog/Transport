import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import { getData, getDataCustom, getFile } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import AddContract from "./AddContract";
import EditContract from "./EditContract";
import DatePicker from "react-datepicker";
import AddPriceTable from "../PriceListManage/AddPriceTable";
import ApproveContract from "./ApproveContract";
import { Tooltip, OverlayTrigger } from "react-bootstrap";

const ContractPage = (props) => {
  const { dataSelected } = props;
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [page, setPage] = useState(1);
  const [perPage, setPerPage] = useState(10);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [selectIdClick, setSelectIdClick] = useState({});

  const [tabIndex, setTabIndex] = useState(0);
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const [priceTable, setPriceTable] = useState([]);
  const [listStatus, setListStatus] = useState([]);
  const [listContractType, setListContractType] = useState([]);
  const [contractType, setContractType] = useState("");
  const [title, setTitle] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <>
          <button
            onClick={() =>
              handleOnclickPriceTable(
                val,
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
          <button
            onClick={() =>
              handleEditButtonClick(
                val,
                SetShowModal("Edit"),
                setTitle("Cập Nhật Hợp Đồng/Phụ Lục")
              )
            }
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Cập Nhật Thông Tin"
          >
            <i className="far fa-edit"></i>
          </button>
          {val.fileContract && val.fileContract !== "0" ? (
            <>
              <button
                onClick={() => handleDownloadContact(val, "contract")}
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Tải File Hợp Đồng"
              >
                <i className="fas fa-download"></i>
              </button>
            </>
          ) : (
            <>
              <button
                disabled={true}
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Tải File Hợp Đồng"
              >
                <i className="fas fa-download"></i>
              </button>
            </>
          )}
          {val.fileCosing && val.fileCosing !== "0" && (
            <>
              <button
                onClick={() => handleDownloadContact(val, "costing")}
                type="button"
                className="btn btn-title btn-sm btn-default mx-1"
                gloss="Tải File Costing"
              >
                <i className="fas fa-file-download"></i>
              </button>
            </>
          )}
        </>
      ),
      width: "170px",
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "Chuỗi",
      selector: (row) => row.chuoiKhachHang,
      allowOverflow: true,
      sortable: true,
    },
    {
      name: <div>Mã Đối Tác</div>,
      selector: (row) => row.maKh,
      sortable: true,
    },
    {
      name: <div>Tên Đối Tác</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.tenKH}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.tenKH}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Mã Hợp Đồng</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.maHopDong}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.maHopDong}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: "Tên Hợp Đồng",
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.tenHienThi}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.tenHienThi}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Loại Hình Hợp Tác</div>,
      selector: (row) => row.loaiHinhHopTac,
    },
    {
      name: <div>Sản Phẩm Dịch Vụ</div>,
      selector: (row) => row.maLoaiSPDV,
    },
    {
      name: <div>Loại Hình Kho</div>,
      selector: (row) => row.maLoaiHinh,
    },
    {
      name: <div>Hình Thức Thuê</div>,
      selector: (row) => row.hinhThucThue,
    },
    {
      name: <div>Trạng thái</div>,
      selector: (row) => row.trangThai,
    },
    {
      name: <div>Thời Gian Bắt Đầu</div>,
      selector: (row) => moment(row.thoiGianBatDau).format(" DD/MM/YYYY"),
      sortable: true,
    },
    {
      name: <div>Thời Gian Kết Thúc</div>,
      selector: (row) => moment(row.thoiGianKetThuc).format(" DD/MM/YYYY"),
      sortable: true,
    },
  ]);

  useEffect(() => {
    setLoading(true);
    (async () => {
      fetchData(1, "", "", "", "", "KH");
      const getListContractType = await getData(`Common/GetListContractType`);
      setListContractType(getListContractType);

      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "Contract",
      ]);
      setListStatus(getStatusList);
    })();

    setLoading(false);
  }, []);

  useEffect(() => {
    if (dataSelected && Object.keys(dataSelected).length > 0) {
      let tabIndex = dataSelected.loaiKH === "KH" ? 0 : 1;
      setTabIndex(tabIndex);
      fetchData(1, dataSelected.tenKh, "", "", "", dataSelected.loaiKH);
    }
  }, [dataSelected]);

  const showModalForm = () => {
    const modal = new Modal(parseExceptionModal.current, {
      keyboard: false,
      backdrop: "static",
    });
    setModal(modal);
    modal.show();
  };

  const handleDownloadContact = async (val, type) => {
    if (val && type) {
      let file = val.fileContract;
      let name = val.tenHienThi;
      if (type === "costing") {
        file = val.fileCosing;
        name = name + "_Costing";
      }
      const getFileDownLoad = await getFile(
        `Contract/DownloadFile?fileId=${file}`,
        name
      );
    }
  };

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

    if (dataSelected && Object.keys(dataSelected).length > 0) {
      KeyWord = dataSelected.tenKh;
      customerType = dataSelected.loaiKH;
    }

    const dataCus = await getData(
      `Contract/GetListContract?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&contractType=${contractType}&customerType=${customerType}`
    );

    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(
      page,
      keySearch,
      !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD"),
      !toDate ? "" : moment(toDate).format("YYYY-MM-DD"),
      "",
      tabIndex === 0 ? "KH" : "NCC"
    );
    setPage(page);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    let customerType = tabIndex === 0 ? "KH" : "NCC";
    let KeyWord = keySearch;

    if (dataSelected && Object.keys(dataSelected).length > 0) {
      KeyWord = dataSelected.tenKh;
      customerType = dataSelected.loaiKH;
    }

    const dataCus = await getData(
      `Contract/GetListContract?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&contractType=${contractType}&customerType=${customerType}`
    );
    setData(dataCus.data);
    setPerPage(newPerPage);
    setLoading(false);
  };

  const hideModal = () => {
    modal.hide();
  };

  const handleSearchClick = async () => {
    await fetchData(
      page,
      keySearch,
      !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD"),
      !toDate ? "" : moment(toDate).format("YYYY-MM-DD"),
      "",
      tabIndex === 0 ? "KH" : "NCC"
    );
  };

  const handleRefeshDataClick = () => {
    fetchData(1, "", "", "", "", "KH");
    setContractType("");
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setTabIndex(0);
  };

  const HandleOnChangeTabs = async (tabIndex) => {
    setTabIndex(tabIndex);
    let customerType = tabIndex === 0 ? "KH" : "NCC";
    await fetchData(1, "", "", "", "", customerType);
  };

  const refeshData = async () => {
    let customerType = tabIndex === 0 ? "KH" : "NCC";
    await fetchData(
      page,
      keySearch,
      fromDate,
      toDate,
      contractType,
      customerType
    );
  };

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
  };

  const ExpandedComponent = ({ data }) => {
    if (data.listAddendums && data.listAddendums.length > 0) {
      return (
        <div className="container-datatable" style={{ height: "50vm" }}>
          <DataTable
            columns={[
              {
                cell: (val) => (
                  <>
                    <button
                      onClick={() =>
                        handleOnclickPriceTable(
                          val,
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
                    <button
                      onClick={() =>
                        handleEditButtonClick(
                          val,
                          SetShowModal("Edit"),
                          setTitle("Cập Nhật Hợp Đồng/Phụ Lục")
                        )
                      }
                      type="button"
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Cập Nhật Thông Tin"
                    >
                      <i className="far fa-edit"></i>
                    </button>
                    {val.fileContract && val.fileContract !== "0" && (
                      <>
                        <button
                          onClick={() => handleDownloadContact(val, "contract")}
                          type="button"
                          className="btn btn-title btn-sm btn-default mx-1"
                          gloss="Tải File Hợp Đồng"
                        >
                          <i className="fas fa-download"></i>
                        </button>
                      </>
                    )}
                    {val.fileCosing && val.fileCosing !== "0" && (
                      <>
                        <button
                          onClick={() => handleDownloadContact(val, "costing")}
                          type="button"
                          className="btn btn-title btn-sm btn-default mx-1"
                          gloss="Tải File Costing"
                        >
                          <i className="fas fa-file-download"></i>
                        </button>
                      </>
                    )}
                  </>
                ),
                width: "268px",
                ignoreRowClick: true,
                allowOverflow: true,
                button: true,
              },
              {
                name: "Account",
                selector: (row) => row.account,
              },
              {
                name: "Mã Hợp Đồng",
                selector: (row) => row.maHopDong,
              },
              {
                name: "Tên Hợp Đồng",
                selector: (row) => (
                  <div className="text-wrap">{row.tenHienThi}</div>
                ),
              },
              {
                name: <div>Loại Hình Hợp Tác</div>,
                selector: (row) => (
                  <div className="text-wrap">{row.loaiHinhHopTac}</div>
                ),
              },
              {
                name: <div>Sản Phẩm Dịch Vụ</div>,
                selector: (row) => (
                  <div className="text-wrap">{row.maLoaiSPDV}</div>
                ),
              },
              {
                name: <div>Loại Hình Kho</div>,
                selector: (row) => (
                  <div className="text-wrap">{row.maLoaiHinh}</div>
                ),
              },
              {
                name: "Hình Thức Thuê",
                selector: (row) => (
                  <div className="text-wrap">{row.hinhThucThue}</div>
                ),
              },
              {
                name: "Trạng thái",
                selector: (row) => row.trangThai,
              },
              {
                name: <div>Thời Gian Bắt Đầu</div>,
                selector: (row) =>
                  moment(row.thoiGianBatDau).format(" DD/MM/YYYY"),
                sortable: true,
              },
              {
                name: <div>Thời Gian Kết Thúc</div>,
                selector: (row) =>
                  moment(row.thoiGianKetThuc).format(" DD/MM/YYYY"),
                sortable: true,
              },
            ]}
            data={data.listAddendums}
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

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản lý hợp đồng</h1>
            </div>
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
                    onClick={() =>
                      showModalForm(
                        SetShowModal("Create"),
                        setTitle("Tạo Mới Hợp Đồng/Phụ Lục")
                      )
                    }
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Duyệt Hợp Đồng/Phụ Lục"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("ApproveContract"),
                        setSelectIdClick({}),
                        setTitle("Duyệt Hợp Đồng/Phụ Lục")
                      )
                    }
                  >
                    <i className="fas fa-check-double"></i>
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
            <Tabs
              selectedIndex={tabIndex}
              onSelect={(index) => HandleOnChangeTabs(index)}
            >
              <TabList>
                {!dataSelected && (
                  <>
                    <Tab>Hợp Đồng Khách Hàng</Tab>
                    <Tab>Hợp Đồng Nhà Cung Cấp</Tab>
                  </>
                )}
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
                    fixedHeaderScrollHeight="55vh"
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
                    fixedHeaderScrollHeight="55vh"
                  />
                </div>
              </TabPanel>
            </Tabs>
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
            style={{ maxWidth: "88%" }}
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
                  {ShowModal === "Edit" && (
                    <EditContract
                      selectIdClick={selectIdClick}
                      getListContract={refeshData}
                      listContractType={listContractType}
                      listStatus={listStatus}
                      hideModal={hideModal}
                      tabIndex={tabIndex}
                    />
                  )}
                  {ShowModal === "Create" && (
                    <AddContract
                      getListContract={refeshData}
                      listContractType={listContractType}
                      listStatus={listStatus}
                      hideModal={hideModal}
                      tabIndex={tabIndex}
                    />
                  )}
                  {ShowModal === "PriceTable" && (
                    <AddPriceTable
                      getListPriceTable={refeshData}
                      selectIdClick={priceTable}
                      listStatus={listStatus}
                    />
                  )}
                  {ShowModal === "ApproveContract" && (
                    <ApproveContract
                      getListContract={refeshData}
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
