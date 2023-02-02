import { useMemo, useState, useEffect, useRef } from "react";
import { getData, getDataCustom, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import { ToastError } from "../Common/FuncToast";
import CreateUser from "./CreateUser";
import UpdateUser from "./UpdateUser";
import RolePage from "../RoleManage/RolePage";
import SetCusForUser from "../RoleManage/SetCusForUser";

const UserPage = () => {
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
  const [toggledClearRows, setToggleClearRows] = useState(false);
  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});
  const [listStatus, setListStatus] = useState([]);
  const [status, setStatus] = useState("");

  const [ShowConfirm, setShowConfirm] = useState(false);
  const [functionSubmit, setFunctionSubmit] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <>
          <button
            onClick={() =>
              handleEditButtonClick(val, SetShowModal("UpdateUser"))
            }
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Cập Nhật Thông Tin "
          >
            <i className="far fa-edit"></i>
          </button>
          <button
            onClick={() =>
              handleEditButtonClick(val, SetShowModal("SetCusForUser"))
            }
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss=""
          >
            <i className="fas fa-user-plus"></i>
          </button>
        </>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "id",
      selector: (row) => row.id,
      omit: true,
    },
    {
      name: "Phân Loại",
      selector: (row) => row.accountType,
      sortable: true,
    },
    {
      name: "Tài Khoản",
      selector: (row) => row.userName,
      sortable: true,
    },
    {
      name: "Họ Và Tên",
      selector: (row) => row.hoVaTen,
      sortable: true,
    },
    {
      name: "Mã Nhân Viên",
      selector: (row) => row.maNhanVien,
    },
    {
      name: "Bộ Phận",
      selector: (row) => row.maBoPhan,
    },
    {
      name: "Phân Quyền",
      selector: (row) => row.roleId,
    },
    {
      name: "Trạng Thái",
      selector: (row) => row.trangThai,
    },
    {
      name: "Người Tạo",
      selector: (row) => row.nguoiTao,
    },
    {
      name: "Thời Cập Nhật",
      selector: (row) => (
        <div className="text-wrap">
          {moment(row.updatedTime).format("DD/MM/YYYY HH:mm:ss")}
        </div>
      ),
      sortable: true,
    },
    {
      name: "Thời gian Tạo",
      selector: (row) => (
        <div className="text-wrap">
          {moment(row.createdTime).format("DD/MM/YYYY HH:mm:ss")}
        </div>
      ),
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

  const handleEditButtonClick = async (val) => {
    var getUser = await getData(`User/GetUser?id=${val.id}`);
    if (getUser && Object.keys(getUser).length > 0) {
      setSelectIdClick(getUser);
      showModalForm();
    }
  };

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const handleClearRows = () => {
    setToggleClearRows(!toggledClearRows);
  };

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    status = ""
  ) => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");
    const dataCus = await getData(
      `User/GetListUser?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`
    );

    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(page);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `User/GetListUser?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`
    );
    setPerPage(newPerPage);
    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const funcAgree = () => {
    if (functionSubmit && functionSubmit.length > 0) {
      switch (functionSubmit) {
        case "Disable":
          return BlockUser();
        case "Delete":
          return DeleteUser();
      }
    }
  };

  const BlockUser = async () => {
    if (
      selectedRows &&
      selectedRows.length > 0 &&
      Object.keys(selectedRows).length > 0
    ) {
      let arr = [];
      selectedRows.map((val) => {
        arr.push(val.id);
      });

      const BlockUser = await postData(`User/BlockUsers`, arr);

      if (BlockUser === 1) {
        fetchData(1);
      }
      setSelectedRows([]);
      handleClearRows();
      setShowConfirm(false);
    }
  };

  const DeleteUser = () => {};

  useEffect(() => {
    setLoading(true);
    (async () => {
      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "Common",
      ]);
      setListStatus(getStatusList);
    })();

    fetchData(1);
    setLoading(false);
  }, []);

  const ShowConfirmDialog = () => {
    if (selectedRows.length < 1) {
      ToastError("Vui lòng chọn người dùng trước đã");
      return;
    } else {
      setShowConfirm(true);
    }
  };

  const handleSearchClick = () => {
    fetchData(1, keySearch, fromDate, toDate, status);
  };

  const handleOnChangeStatus = (value) => {
    setLoading(true);
    setStatus(value);
    fetchData(1, keySearch, fromDate, toDate, value);
    setLoading(false);
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
              <h1>Quản Lý Người Dùng</h1>
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
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Tạo Mới Người Dùng"
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
                    title="Create Role"
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Quản Lý Role"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("RolePage"),
                        setSelectIdClick({})
                      )
                    }
                  >
                    <i className="fas fa-user-tag"></i>
                  </button>
                  <button
                    title="Block User"
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Khóa Người Dùng"
                    onClick={() => {
                      setFunctionSubmit("Disable");
                      ShowConfirmDialog();
                    }}
                  >
                    <i className="fas fa-eye-slash"></i>
                  </button>
                  <button
                    title="Delete User"
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Xóa Người Dùng"
                    onClick={() => {
                      setFunctionSubmit("Delete");
                      ShowConfirmDialog();
                    }}
                  >
                    <i className="fas fa-trash-alt"></i>
                  </button>
                </div>
                <div className="col col-sm">
                  <div className="row">
                    <div className="col col-sm"></div>
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
                title="Danh sách Người Dùng"
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                selectableRows
                clearSelectedRows={toggledClearRows}
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
            style={{ maxWidth: "80%" }}
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
                    <CreateUser getListUser={fetchData} />
                  )}
                  {ShowModal === "UpdateUser" && (
                    <UpdateUser
                      getListUser={fetchData}
                      selectIdClick={selectIdClick}
                      hideModal={hideModal}
                    />
                  )}
                  {ShowModal === "SetCusForUser" && (
                    <SetCusForUser
                      selectIdClick={selectIdClick}
                      hideModal={hideModal}
                    ></SetCusForUser>
                  )}
                  {ShowModal === "RolePage" && <RolePage />}
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

export default UserPage;
