import { useMemo, useState, useEffect, useRef } from "react";
import { getData, getDataCustom, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import CreateRole from "./CreateRole";
import UpdateRole from "./UpdateRole";

const RolePage = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("UpdateRole"))}
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
      name: "id",
      selector: (row) => row.id,
      omit: true,
    },
    {
      name: "Tên Phân Quyền",
      selector: (row) => row.roleName,
      sortable: true,
    },
    {
      name: "Thời Cập Nhật",
      selector: (row) => moment(row.updatedTime).format("DD-MM-YYYY HH:mm:ss"),
      sortable: true,
    },
    {
      name: "Thời gian Tạo",
      selector: (row) => moment(row.createdTime).format("DD-MM-YYYY HH:mm:ss"),
      sortable: true,
    },
  ]);

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [selectIdClick, setSelectIdClick] = useState({});
  const [listStatus, setListStatus] = useState([]);

  useEffect(() => {
    setLoading(true);
    (async () => {
      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "Common",
      ]);
      setListStatus(getStatusList);
    })();

    fetchData();
    setLoading(false);
  }, []);

  const handleEditButtonClick = async (val) => {
    var getRole = await getData(`User/GetTreePermission?id=${val.id}`);
    if (getRole && Object.keys(getRole).length > 0) {
      setSelectIdClick(getRole);
      showModalForm();
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

  const fetchData = async () => {
    setLoading(true);
    const dataCus = await getData(`User/GetListRoleSelect`);

    formatTable(dataCus);
    setLoading(false);
  };

  function formatTable(data) {
    data.map((val) => {
      !val.createdTime
        ? (val.createdDate = "")
        : (val.createdDate = moment(val.createdDate).format("DD/MM/YYYY"));

      !val.updatedTime
        ? (val.updatedDate = "")
        : (val.updatedDate = moment(val.updatedDate).format("DD/MM/YYYY"));
    });
    setData(data);
  }

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản Lý Phân Quyền</h1>
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
                    onClick={() => showModalForm(SetShowModal("Create"))}
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                </div>
                <div className="col col-sm">
                  <div className="row">
                    <div className="col col-sm"></div>
                    <div className="col col-sm"></div>
                  </div>
                </div>
                <div className="col col-sm"></div>
                <div className="col col-sm "></div>
              </div>
            </div>
          </div>
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                columns={columns}
                data={data}
                progressPending={loading}
                highlightOnHover
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
            style={{ maxWidth: "40%" }}
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
                    <CreateRole getListRole={fetchData} />
                  )}
                  {ShowModal === "UpdateRole" && (
                    <UpdateRole
                      getListRole={fetchData}
                      selectIdClick={selectIdClick}
                      hideModal={hideModal}
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

export default RolePage;
