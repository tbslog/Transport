import { useMemo, useState, useEffect, useRef } from "react";
import { getData, postData, getDataCustom } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import { ToastError } from "../Common/FuncToast";
import DatePicker from "react-datepicker";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import EditContract from "./EditContract";
import ApprovePriceTable from "../PriceListManage/ApprovePriceTable";

const ApproveContract = (props) => {
  const { getListContract, checkShowModal } = props;

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [keySearch, setKeySearch] = useState("");
  const [title, setTitle] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});
  const [toggledClearRows, setToggleClearRows] = useState(false);

  const [isAccept, setIsAccept] = useState();
  const [ShowConfirm, setShowConfirm] = useState(false);

  const [listStatus, setListStatus] = useState([]);
  const [listContractType, setListContractType] = useState([]);

  const [priceTable, setPriceTable] = useState([]);

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
        </>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "Chuỗi",
      selector: (row) => <div className="text-wrap">{row.chuoiKhachHang}</div>,
    },
    {
      name: <div>Mã Đối Tác</div>,
      selector: (row) => row.maKh,
    },
    {
      name: <div>Tên Đối Tác</div>,
      selector: (row) => <div className="text-wrap">{row.tenKH}</div>,
    },
    {
      name: <div>Mã Hợp Đồng</div>,
      selector: (row) => <div className="text-wrap">{row.maHopDong}</div>,
    },
    {
      name: <div>Tên Hợp Đồng</div>,
      selector: (row) => <div className="text-wrap">{row.tenHienThi}</div>,
    },
    {
      name: <div>Loại Hình Hợp Tác</div>,
      selector: (row) => <div className="text-wrap">{row.loaiHinhHopTac}</div>,
    },
    {
      name: <div>Sản Phẩm Dịch Vụ</div>,
      selector: (row) => <div className="text-wrap">{row.maLoaiSPDV}</div>,
    },
    {
      name: <div>Loại Hình Kho</div>,
      selector: (row) => <div className="text-wrap">{row.maLoaiHinh}</div>,
    },
    {
      name: <div>Hình Thức Thuê</div>,
      selector: (row) => <div className="text-wrap">{row.hinhThucThue}</div>,
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
    (async () => {
      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "Contract",
      ]);
      setListStatus(getStatusList);
      const getListContractType = await getData(`Common/GetListContractType`);
      setListContractType(getListContractType);
      fetchData(1);
    })();
  }, []);

  const handleEditButtonClick = async (val) => {
    if (val && Object.keys(val).length > 0) {
      const getDataContract = await getData(
        `Contract/GetContractById?Id=${val.maHopDong}`
      );
      setSelectIdClick(getDataContract);
      showModalForm();
    }
  };

  const AcceptContract = async (isAccept) => {
    if (
      selectedRows &&
      selectedRows.length > 0 &&
      Object.keys(selectedRows).length > 0
    ) {
      let arr = [];
      selectedRows.map((val) => {
        arr.push({
          contractId: val.maHopDong,
          Selection: isAccept,
        });
      });

      const SetApprove = await postData(`Contract/ApproveContract`, arr);
      setShowConfirm(false);
      setSelectedRows([]);
      handleClearRows();

      if (SetApprove === 1) {
        fetchData(1);
        getListContract(1);
      }
    }
  };

  const handleOnclickPriceTable = async (val) => {
    setPriceTable(val);
    showModalForm();
  };

  const funcAgree = () => {
    if (selectedRows && selectedRows.length > 0) {
      AcceptContract(isAccept);
    }
  };

  const ShowConfirmDialog = () => {
    if (selectedRows.length < 1) {
      ToastError("Vui lòng chọn phụ phí để duyệt");
      return;
    } else {
      setShowConfirm(true);
    }
  };

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const handleClearRows = () => {
    setToggleClearRows(!toggledClearRows);
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

  const fetchData = async (page, KeyWord = "", fromDate = "", toDate = "") => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }

    const dataCus = await getData(
      `Contract/GetListContractApprove?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}`
    );

    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(page, keySearch, fromDate, toDate);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `Contract/GetListContractApprove?PageNumber=${page}&PageSize=${perPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}`
    );
    setLoading(false);
    setPerPage(newPerPage);
    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
  };

  const handleSearchClick = () => {
    fetchData(1, keySearch, fromDate, toDate);
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
      <section className="content">
        <div className="card">
          <div className="card-header">
            <div className="container-fruid">
              <div className="row">
                <div className="col col-sm">
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default "
                    gloss="Duyệt Hợp Đồng/Phụ Lục"
                    onClick={() => {
                      ShowConfirmDialog();
                      setIsAccept(0);
                    }}
                  >
                    <i className="fas fa-thumbs-up"></i>
                  </button>
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-4"
                    gloss="Không Duyệt Hợp Đồng/Phụ Lục"
                    onClick={() => {
                      ShowConfirmDialog();
                      setIsAccept(1);
                    }}
                  >
                    <i className="fas fa-thumbs-down"></i>
                  </button>
                </div>
                <div className="col col-sm"></div>
                <div className="col col-sm">
                  <div className="row">
                    <div className="col col-sm"></div>
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
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                title="Danh Sách Hợp Đồng/Phụ Lục Chờ Duyệt"
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                selectableRows
                paginationServer
                paginationTotalRows={totalRows}
                onSelectedRowsChange={handleChange}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                clearSelectedRows={toggledClearRows}
                highlightOnHover
                striped
                direction="auto"
                responsive
                fixedHeader
                fixedHeaderScrollHeight="60vh"
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
                  {ShowModal === "Edit" && (
                    <EditContract
                      selectIdClick={selectIdClick}
                      getListContract={fetchData}
                      listContractType={listContractType}
                      listStatus={listStatus}
                      hideModal={hideModal}
                      tabIndex={0}
                    />
                  )}
                  {ShowModal === "PriceTable" && (
                    <ApprovePriceTable
                      checkShowModal={modal}
                      contractId={priceTable.maHopDong}
                    />
                  )}
                </>
              </div>
            </div>
          </div>
        </div>
      </section>
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

export default ApproveContract;
