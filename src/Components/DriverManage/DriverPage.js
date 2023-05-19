import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import {
  getData,
  postFile,
  getDataCustom,
  postData,
} from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import { ToastWarning } from "../Common/FuncToast";
import CreateDriver from "./CreateDriver";
import UpdateDriver from "./UpdateDriver";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";

const DriverPage = () => {
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
  const [listStatus, setListStatus] = useState([]);

  const [isAccept, setIsAccept] = useState();
  const [ShowConfirm, setShowConfirm] = useState(false);

  const [title, setTitle] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <>
          <button
            onClick={() =>
              handleEditButtonClick(
                val,
                SetShowModal("Edit"),
                setTitle("Cập Nhật Thông Tin Tài Xế")
              )
            }
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Cập Nhật Thông Tin"
          >
            <i className="far fa-edit"></i>
          </button>
          <button
            onClick={() =>
              ShowConfirmDialog(
                val,
                SetShowModal("CreateAccount"),
                setTitle("Tạo Tài Khoản Tài Xế")
              )
            }
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Tạo Tài Khoản Tài Xế"
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
      name: "Đơn Vị Vận Tải",
      selector: (row) => row.tenDonViVanTai,
    },
    {
      name: "Mã Tài Xế",
      selector: (row) => row.maTaiXe,
    },
    {
      name: "Số Điện Thoại",
      selector: (row) => row.soDienThoai,
    },
    {
      name: "CCCD",
      selector: (row) => row.cccd,
    },
    {
      name: "Họ Và Tên",
      selector: (row) => row.hoVaTen,
    },
    {
      name: "Ngày Sinh",
      selector: (row) => row.ngaySinh,
    },

    {
      name: "Đơn Vị Quản Lý",
      selector: (row) => row.maNhaCungCap,
    },
    {
      name: "Thời Gian cập nhật",
      selector: (row) => row.updatedtime,
      sortable: true,
    },
    {
      name: "Thời Gian tạo mới",
      selector: (row) => row.createdtime,
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
    var getById = await getData(`Driver/getDriverById?driverId=${val.maTaiXe}`);
    setSelectIdClick(getById);
  };

  const fetchData = async (page, KeyWord = "") => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }

    const dataCus = await getData(
      `Driver/GetListDriver?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}`
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
      `Driver/GetListDriver?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}`
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
      let dataCus = await getData(
        `Driver/GetListDriver?PageNumber=1&PageSize=10`
      );
      formatTable(dataCus.data);
      setTotalRows(dataCus.totalRecords);

      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "common",
      ]);
      setListStatus(getStatusList);
    })();

    setLoading(false);
  }, []);

  function formatTable(data) {
    data.map((val) => {
      val.createdtime = moment(val.createdtime).format("DD/MM/YYYY HH:mm:ss");
      val.updatedtime = moment(val.updatedtime).format("DD/MM/YYYY HH:mm:ss");
      val.ngaySinh = moment(val.ngaySinh).format("DD/MM/YYYY");
    });
    setData(data);
  }

  const handleSearchClick = async () => {
    if (keySearch === "") {
      ToastWarning("Vui lòng nhập thông tin tìm kiếm");
      return;
    }
    await fetchData(1, keySearch);
  };

  const handleRefeshDataClick = async () => {
    setKeySearch("");
    await fetchData(1);
  };

  const ShowConfirmDialog = (val) => {
    setSelectIdClick(val);
    setShowConfirm(true);
  };

  const funcAgree = async () => {
    switch (ShowModal) {
      case "CreateAccount":
        const createAcc = await postData(
          `Driver/CreateAccountDriver?driverId=${selectIdClick.maTaiXe}`
        );

        if (createAcc === 1) {
          fetchData(1);
        }
        setShowConfirm(false);
        break;
      default:
        return;
    }
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản Lý Tài Xế</h1>
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
                    gloss="Tạo Mới Tài Xế"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("Create"),
                        setTitle("Tạo Mới Thông Tin Tài Xế")
                      )
                    }
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                </div>
                <div className="col-sm-3"></div>
                <div className="col-sm-3"></div>
                <div className="col-sm-3 ">
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
                title="Danh sách Tài Xế"
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
                fixedHeader
                fixedHeaderScrollHeight="60vh"
              />
            </div>
          </div>
          <div className="card-footer">
            <div className="row">
              <div className="col-sm-3"></div>
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
            style={{ maxWidth: "80%" }}
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
                    <UpdateDriver
                      selectIdClick={selectIdClick}
                      listStatus={listStatus}
                      getListDriver={fetchData}
                      hideModal={hideModal}
                    />
                  )}
                  {ShowModal === "Create" && (
                    <CreateDriver
                      getListDriver={fetchData}
                      listStatus={listStatus}
                    />
                  )}
                </>
              </div>
            </div>
          </div>
        </div>
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
      </section>
    </>
  );
};

export default DriverPage;
