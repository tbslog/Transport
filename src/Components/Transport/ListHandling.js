import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postFile, getDataCustom } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import UpdateHandling from "./UpdateHandling";

const ListHandling = (props) => {
  const { dataClick } = props;

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();
  const [selectIdClick, setSelectIdClick] = useState({});
  const [IsLoading, SetIsLoading] = useState(false);

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("Edit"))}
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
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("Attachment"))}
          type="button"
          className="btn btn-sm btn-default"
        >
          <i className="fas fa-file-image"></i>
        </button>
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
      name: "Mã Số Xe",
      selector: (row) => row.maSoXe,
      sortable: true,
    },

    {
      name: "Tài Xế",
      selector: (row) => row.tenTaiXe,
      sortable: true,
    },
    {
      name: "Loại Phương Tiện",
      selector: (row) => row.ptVanChuyen,
      sortable: true,
    },
    {
      name: "KhoiLuong",
      selector: (row) => row.khoiLuong,
      sortable: true,
    },
    {
      name: "TheTich",
      selector: (row) => row.theTich,
      sortable: true,
    },
    {
      name: "Thời Gian Lấy Hàng",
      selector: (row) => moment(row.thoiGianLayHang).format("DD/MM/YYYY HH:mm"),
      sortable: true,
    },
    {
      name: "Thời Gian Trả Hàng",
      selector: (row) => moment(row.thoiGianTraHang).format("DD/MM/YYYY HH:mm"),
      sortable: true,
    },
    {
      name: "Trạng Thái",
      selector: (row) => row.trangThai,
      sortable: true,
    },
    {
      name: "Thời Gian Lập Đơn",
      selector: (row) =>
        moment(row.thoiGianTaoDon).format("DD/MM/YYYY HH:mm:ss"),
      sortable: true,
    },
  ]);

  useEffect(() => {
    if (props && dataClick && Object.keys(dataClick).length > 0) {
      fetchData(dataClick.maVanDon);
    }
  }, [props, dataClick]);

  const fetchData = async (transportId) => {
    setLoading(true);
    const datatransport = await getData(
      `BillOfLading/GetListHandlingByTransportId?transportId=${transportId}`
    );
    setData(datatransport);
    setLoading(false);
  };

  const handleEditButtonClick = (value) => {
    setSelectIdClick(value);
    showModalForm();
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

  return (
    <>
      <section className="content">
        <div className="card">
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                title={"Mã Vận Đơn: " + dataClick.maVanDon}
                columns={columns}
                data={data}
                progressPending={loading}
                highlightOnHover
              />
            </div>
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
            style={{ maxWidth: "90%" }}
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
                    <UpdateHandling
                      getlistData={fetchData}
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

export default ListHandling;
