import { Modal } from "bootstrap";
import SplitPane, { Pane, SashContent } from "split-pane-react";
import "split-pane-react/esm/themes/default.css";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import DatePicker from "react-datepicker";
import moment from "moment";
import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, getDataCustom } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import CreateTransportLess from "./CreateTransportLess";
import UpdateTransport from "./UpdateTransport";
import AddSubFeeByHandling from "./AddSubFeeByHandling";
import ApproveSubFeeByHandling from "./ApproveSubFeeByHandling";

function style(color) {
  return {
    height: "100%",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: color,
  };
}

const HandlingPageNew = () => {
  const [sizes, setSizes] = useState([100, 100]);
  const [sizesH, setSizesH] = useState([100, 100]);
  const [sizesV, setSizesV] = useState([100, 100]);

  const TransportColumns = useMemo(() => [
    {
      cell: (val) => (
        <button
          onClick={() =>
            handleEditButtonClickTransport(val, SetShowModal("EditTransport"))
          }
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
    {
      name: <div>Mã Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.maVanDon}</div>,
    },
    {
      name: <div>Loại Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.loaiVanDon}</div>,
    },
    {
      name: <div>Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.tenKH}</div>,
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
    // {
    //   name: <div>Thời Gian Lấy/Trả Rỗng</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {moment(row.thoiGianLayTraRong).format("DD/MM/YYYY HH:mm")}
    //     </div>
    //   ),
    //   sortable: true,
    // },
    // {
    //   name: <div>Thời Gian Hạn Lệnh</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {moment(row.thoiGianHanLenh).format("DD/MM/YYYY HH:mm")}
    //     </div>
    //   ),
    //   sortable: true,
    // },
    {
      name: <div>Thời Gian Lấy Hàng</div>,
      selector: (row) => (
        <div className="text-wrap">
          {moment(row.thoiGianLayHang).format("DD/MM/YYYY HH:mm")}
        </div>
      ),
      sortable: true,
    },
    {
      name: <div>Thời Gian Trả Hàng</div>,
      selector: (row) => (
        <div className="text-wrap">
          {moment(row.thoiGianTraHang).format("DD/MM/YYYY HH:mm")}
        </div>
      ),
      sortable: true,
    },
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
    // {
    //   name: <div>Thời Gian Lập Đơn</div>,

    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {moment(row.thoiGianTaoDon).format("DD/MM/YYYY HH:mm")}
    //     </div>
    //   ),
    //   sortable: true,
    // },
  ]);

  const [loading, setLoading] = useState(false);
  const [ShowConfirm, setShowConfirm] = useState(false);
  const [funcName, setFuncName] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [dataTransport, setDataTransport] = useState([]);
  const [selectIdClickTransport, setSelectIdClickTransport] = useState({});
  const [totalRowsTransport, setTotalRowsTransport] = useState(0);
  const [selectedRowsTransport, setSelectedRowsTransport] = useState([]);
  const [perPageTransport, setPerPageTransport] = useState(10);
  const [pageTransport, setPageTransport] = useState(1);
  const [keySearchTransport, setKeySearchTransport] = useState("");
  const [listStatusTransport, setListStatusTransport] = useState([]);
  const [statusTransport, setStatusTransport] = useState("");
  const [fromDateTransport, setFromDateTransport] = useState("");
  const [toDateTransport, setToDateTransport] = useState("");
  const [toggledClearRowsTransport, setToggleClearRowsTransport] =
    useState(false);

  console.log(selectedRowsTransport);

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
      setListStatusTransport(getStatusList);
      fetchDataTransport(1);
    })();
  }, []);

  const fetchDataTransport = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    status = ""
  ) => {
    setLoading(true);
    const datatransport = await getData(
      `BillOfLading/GetListTransport?PageNumber=${page}&PageSize=${perPageTransport}&KeyWord=${KeyWord}&StatusId=${status}&fromDate=${fromDate}&toDate=${toDate}&transportType=LESS`
    );

    setDataTransport(datatransport.data);
    setTotalRowsTransport(datatransport.totalRecords);
    setLoading(false);
  };

  const handlePageChangeTransport = async (page) => {
    setPageTransport(page);
    fetchDataTransport(
      page,
      keySearchTransport,
      !fromDateTransport ? "" : moment(fromDateTransport).format("YYYY-MM-DD"),
      !toDateTransport ? "" : moment(toDateTransport).format("YYYY-MM-DD"),
      statusTransport
    );
  };

  const handlePerRowsChangeTransport = async (newPerPage, page) => {
    setLoading(true);

    const datatransport = await getData(
      `BillOfLading/GetListTransport?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearchTransport}&StatusId=${statusTransport}&fromDate=${fromDateTransport}&toDate=${toDateTransport}&transportType=LESS`
    );
    setDataTransport(datatransport);
    setPerPageTransport(newPerPage);
    setLoading(false);
  };

  const handleChangeTransport = useCallback((state) => {
    setSelectedRowsTransport(state.selectedRows);
  }, []);

  const handleEditButtonClickTransport = async (value) => {
    let getTransportById = await getData(
      `BillOfLading/GetTransportById?transportId=${value.maVanDon}`
    );
    setSelectIdClickTransport(getTransportById);
    showModalForm();
  };

  const handleSearchClick = () => {
    fetchDataTransport(
      1,
      keySearchTransport,
      !fromDateTransport ? "" : moment(fromDateTransport).format("YYYY-MM-DD"),
      !toDateTransport ? "" : moment(toDateTransport).format("YYYY-MM-DD"),
      statusTransport
    );
  };

  const handleOnChangeStatus = (val) => {
    setStatusTransport(val);
    fetchDataTransport(
      1,
      keySearchTransport,
      fromDateTransport,
      toDateTransport,
      val
    );
  };

  const handleRefeshDataClick = () => {
    setKeySearchTransport("");
    setFromDateTransport("");
    setToDateTransport("");
    fetchDataTransport(1);
  };

  const handleClearRowsTransport = () => {
    setToggleClearRowsTransport(!toggledClearRowsTransport);
  };

  const funcAgree = () => {
    if (selectedRowsTransport && selectIdClickTransport.length > 0) {
      handleClearRowsTransport();
    }
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản Lý Điều Phối LTL/LCL</h1>
            </div>
          </div>
        </div>
      </section>

      <section className="content">
        <div className="card">
          <div className="card-body">
            <div className="demo-wrap">
              <div style={{ height: "77vh" }}>
                <SplitPane
                  sizes={sizes}
                  onChange={setSizes}
                  resizerSize={8}
                  sashRender={() => (
                    <SashContent
                      style={{ backgroundColor: "rgba(0,0,0,.2)" }}
                    />
                  )}
                >
                  <Pane minSize="20%" maxSize="70%">
                    <div style={{ height: "100%" }}>
                      <SplitPane
                        split="horizontal"
                        sizes={sizesH}
                        onChange={setSizesH}
                        sashRender={() => (
                          <SashContent
                            style={{ backgroundColor: "rgba(0,0,0,.2)" }}
                          />
                        )}
                      >
                        <Pane minSize="20%" maxSize="70%">
                          <div style={style("#fff")}>Bản Đồ</div>
                        </Pane>
                        <Pane>
                          <div style={{ height: "100%" }}>
                            <SplitPane
                              sizes={sizesV}
                              onChange={setSizesV}
                              sashRender={() => (
                                <SashContent
                                  style={{ backgroundColor: "rgba(0,0,0,.2)" }}
                                />
                              )}
                            >
                              <Pane minSize="20%" maxSize="70%">
                                <div style={style("#fff")}>Tài Xế</div>
                              </Pane>
                              <Pane>
                                <div style={style("#fff")}>Xe Vận Chuyển</div>
                              </Pane>
                            </SplitPane>
                          </div>
                        </Pane>
                      </SplitPane>
                    </div>
                  </Pane>
                  <Pane>
                    <div style={style("#fff")}>
                      <div className="card card-default">
                        <div className="card-header">
                          <div className="container-fruid">
                            <div className="row">
                              <button
                                type="button"
                                className="btn btn-title btn-sm btn-default mx-1"
                                gloss="Tạo Vận Đơn LCL/LTL "
                                onClick={() =>
                                  showModalForm(SetShowModal("CreateLCL/LTL"))
                                }
                              >
                                <i className="fas fa-plus-circle"></i>
                              </button>
                              <div className="col col-sm">
                                <button
                                  className="btn btn-title btn-sm btn-default mx-1"
                                  gloss="Duyệt Phụ Phí"
                                  type="button"
                                  onClick={() =>
                                    showModalForm(
                                      SetShowModal("ApproveSubFee"),
                                      setSelectIdClickTransport({})
                                    )
                                  }
                                >
                                  <i className="fas fa-check-double"></i>
                                </button>
                              </div>
                              <div className="col col-sm">
                                <div className="row">
                                  <div className="col col-sm"></div>
                                  <div className="col col-sm">
                                    <div className="input-group input-group-sm">
                                      <select
                                        className="form-control form-control-sm"
                                        onChange={(e) =>
                                          handleOnChangeStatus(e.target.value)
                                        }
                                        value={statusTransport}
                                      >
                                        <option value="">
                                          Tất Cả Trạng Thái
                                        </option>
                                        {listStatusTransport &&
                                          listStatusTransport.map((val) => {
                                            return (
                                              <option
                                                value={val.statusId}
                                                key={val.statusId}
                                              >
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
                                        selected={fromDateTransport}
                                        onChange={(date) =>
                                          setFromDateTransport(date)
                                        }
                                        dateFormat="dd/MM/yyyy"
                                        className="form-control form-control-sm"
                                        placeholderText="Từ ngày"
                                        value={fromDateTransport}
                                      />
                                    </div>
                                  </div>
                                  <div className="col col-sm">
                                    <div className="input-group input-group-sm">
                                      <DatePicker
                                        selected={toDateTransport}
                                        onChange={(date) =>
                                          setToDateTransport(date)
                                        }
                                        dateFormat="dd/MM/yyyy"
                                        className="form-control form-control-sm"
                                        placeholderText="Đến Ngày"
                                        value={toDateTransport}
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
                                    value={keySearchTransport}
                                    onChange={(e) =>
                                      setKeySearchTransport(e.target.value)
                                    }
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
                          <div
                            className="container-datatable"
                            style={{ height: "50vm" }}
                          >
                            <DataTable
                              title="Danh sách vận đơn"
                              direction="auto"
                              responsive
                              columns={TransportColumns}
                              data={dataTransport}
                              progressPending={loading}
                              pagination
                              paginationServer
                              paginationTotalRows={totalRowsTransport}
                              onSelectedRowsChange={handleChangeTransport}
                              onChangeRowsPerPage={handlePerRowsChangeTransport}
                              onChangePage={handlePageChangeTransport}
                              clearSelectedRows={toggledClearRowsTransport}
                              selectableRows
                              highlightOnHover
                              striped
                            />
                          </div>
                        </div>
                      </div>
                    </div>
                  </Pane>
                </SplitPane>
              </div>
            </div>
          </div>
          <div className="card-footer"></div>
        </div>
        <div>
          {ShowConfirm === true && (
            <ConfirmDialog
              setShowConfirm={setShowConfirm}
              title={"Bạn có chắc chắn với quyết định này?"}
              content={
                "Khi thực hiện hành động này sẽ không thể hoàn tác lại được nữa."
              }
              // funcAgree={funcAgree}
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
                <div className="modal-body">
                  <>
                    {/* {ShowModal === "addSubFee" && (
                    <AddSubFeeByHandling dataClick={selectIdClick} />
                  )}
                  {ShowModal === "ApproveSubFee" && (
                    <ApproveSubFeeByHandling CheckModalShow={modal} />
                  )} */}
                    {ShowModal === "CreateLCL/LTL" && (
                      <CreateTransportLess
                        getListTransport={fetchDataTransport}
                      />
                    )}
                    {ShowModal === "EditTransport" && (
                      <UpdateTransport
                        getListTransport={fetchDataTransport}
                        selectIdClick={selectIdClickTransport}
                        hideModal={hideModal}
                      />
                    )}
                    {ShowModal === "addSubFee" && (
                      <AddSubFeeByHandling dataClick={selectIdClickTransport} />
                    )}
                    {ShowModal === "ApproveSubFee" && (
                      <ApproveSubFeeByHandling CheckModalShow={modal} />
                    )}
                  </>
                </div>
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

export default HandlingPageNew;
