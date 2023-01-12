import { useMemo, useState, useEffect, useRef } from "react";
import { getData, getDataCustom, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import Cookies from "js-cookie";

const HandlingByTransport = (props) => {
  const { dataClick, refeshData } = props;
  const accountType = Cookies.get("AccType");

  const [data, setData] = useState([]);
  const parseExceptionModal = useRef();
  const [selectIdClick, setSelectIdClick] = useState({});
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [page, setPage] = useState(1);

  const [loading, setLoading] = useState(false);
  const [ShowConfirm, setShowConfirm] = useState(false);
  const [funcName, setFuncName] = useState("");
  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);

  const [keySearch, setKeySearch] = useState("");
  const [listStatus, setListStatus] = useState([]);
  const [status, setStatus] = useState("");

  const [transportId, setTransportId] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <>
          {accountType && accountType === "KH" && (
            <div>
              <>
                {(val.statusId === 30 || !val.statusId) && (
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
                )}
              </>
            </div>
          )}
        </>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      selector: (row) => row.maDieuPhoi,
      omit: true,
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
      name: "statusId",
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

  useEffect(() => {
    setLoading(true);
    (async () => {
      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "Handling",
      ]);
      setListStatus(getStatusList);
    })();
    setLoading(false);
  }, []);

  const showConfirmDialog = (val) => {
    setSelectIdClick(val);
    setShowConfirm(true);
  };

  useEffect(() => {
    if (dataClick && Object.keys(dataClick).length > 0) {
      setTransportId(dataClick.maVanDon);
      fetchData(dataClick.maVanDon, 1);
    }
  }, [dataClick, listStatus]);

  const funcAgree = () => {
    if (funcName && funcName.length > 0) {
      switch (funcName) {
        case "CancelHandling":
          return setCancelHandling();
      }
    }
  };

  const handlePageChange = async (page) => {
    setPage(page);
    fetchData(transportId, page, keySearch, status);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `BillOfLading/GetListHandlingByTransportId?transportId=${transportId}&PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&statusId=${status}`
    );
    setPerPage(newPerPage);
    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const fetchData = async (transportId, page, KeyWord = "", status = "") => {
    setLoading(true);
    console.log(transportId);
    const dataCus = await getData(
      `BillOfLading/GetListHandlingByTransportId?transportId=${transportId}&PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&statusId=${status}`
    );

    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const setCancelHandling = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      var update = await postData(
        `BillOfLading/CancelHandlingByCus?id=${
          !selectIdClick.maDieuPhoi ? 0 : selectIdClick.maDieuPhoi
        }&transportId=${transportId}`
      );

      if (update === 1) {
        fetchData(transportId, page, keySearch, status);
        refeshData();
        setShowConfirm(false);
      } else {
        setShowConfirm(false);
      }
    }
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

  const handleSearchClick = () => {
    fetchData(transportId, page, keySearch, status);
  };

  const handleOnChangeStatus = (value) => {
    setStatus(value);
    fetchData(transportId, page, keySearch, value);
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setStatus("");
    setPerPage(10);
    fetchData(transportId, 1);
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản Lý Điều Phối</h1>
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
                  <div className="row">
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
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                paginationRowsPerPageOptions={[10, 30, 50, 100]}
                paginationTotalRows={totalRows}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
                striped
                direction="auto"
                responsive
              />
            </div>
          </div>
          <div className="card-footer">
            <div className="row">
              <div className="col-sm-3">
                {/* <button
                  // href={FileExcelImport}
                  onClick={() => handleExportExcel()}
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Tải File Excel"
                  type="button"
                >
                  <i className="fas fa-file-excel"></i>
                </button> */}
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
              funcAgree={funcAgree}
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
                <div className="modal-body"></div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
};

export default HandlingByTransport;
