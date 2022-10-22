import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData, postFile } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import UpdateHandling from "./UpdateHandling";
import HandlingImage from "./HandlingImage";

const ListHandling = (props) => {
  const { dataClick } = props;

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();
  const [selectIdClick, setSelectIdClick] = useState({});

  const columns = useMemo(() => [
    {
      name: "Chỉnh Sửa",
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
      name: "Xem Hình",
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("Image"))}
          type="button"
          className="btn btn-sm btn-default"
        >
          <i className="fas fa-images"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "Đẩy Hình",
      cell: (val) => (
        <div className="upload-btn-wrapper">
          <button className="btn btn-sm btn-default mx-1">
            <i className="fas fa-file-upload"></i>
          </button>
          <input
            type="file"
            name="myfile"
            multiple
            accept="image/png, image/jpg, image/jpeg"
            onChange={(e) => handleUploadImage(val, e)}
          />
        </div>
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
      name: "Mã Vận Đơn",
      selector: (row) => row.maVanDon,
      sortable: true,
    },
    {
      name: "Loại Vận Đơn",
      selector: (row) => row.phanLoaiVanDon,
      sortable: true,
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
      name: "Khối Lượng",
      selector: (row) => row.khoiLuong,
      sortable: true,
    },
    {
      name: "Thể Tích",
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

  const handleUploadImage = async (val, e) => {
    let files = e.target.files;
    const transportId = val.maVanDon;
    const handlingId = val.maDieuPhoi;

    let arrfiles = [];

    for (let i = 0; i <= files.length - 1; i++) {
      arrfiles.push(files[i]);
    }

    const uploadFiles = await postFile("BillOfLading/UploadFile", {
      files: arrfiles,
      transportId: transportId,
      handlingId: handlingId,
    });
  };

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
                  {ShowModal === "Image" && (
                    <HandlingImage
                      dataClick={selectIdClick}
                      hideModal={hideModal}
                      checkModal={modal}
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
