import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData, getFileImage } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";

const HandlingImage = (props) => {
  const { dataClick, checkModal } = props;

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();
  const [selectIdClick, setSelectIdClick] = useState({});
  const [image, setImage] = useState("");

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

  const handleDeleteImage = async (val) => {
    const fileId = val.id;
    var del = await postData(`BillOfLading/DeleteImage?fileId=${fileId}`);

    if (del === 1) {
      let datafilter = data.filter((x) => x.id !== fileId);
      setData(datafilter);
    }
  };

  const columns = useMemo(() => [
    {
      //   name: "Xem Hình",
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("ShowImage"))}
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Xem Hình Ảnh"
        >
          <i className="far fa-eye"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      //   name: "Xóa Hình",
      cell: (val) => (
        <button
          onClick={() => handleDeleteImage(val)}
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Xóa Hình Ảnh"
        >
          <i className="fas fa-trash"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },

    {
      selector: (row) => row.id,
      omit: true,
    },
    {
      //   name: "Tên Hình",
      selector: (row) => row.fileName,
      sortable: true,
    },
    {
      name: "",
      selector: (row) => row.fileType,
      sortable: true,
    },
    {
      //   name: "Thời Gian Upload",
      selector: (row) => moment(row.uploadedTime).format("DD/MM/YYYY HH:mm:ss"),
      sortable: true,
    },
  ]);

  useEffect(() => {
    if (props && dataClick && Object.keys(dataClick).length > 0) {
      fetchData(dataClick.maDieuPhoi);
    }
  }, [props, dataClick, checkModal]);

  const fetchData = async (maDieuPhoi) => {
    setLoading(true);
    const data = await getData(
      `BillOfLading/GetListImage?handlingId=${maDieuPhoi}`
    );

    setData(data);
    setLoading(false);
  };

  const handleEditButtonClick = (value) => {
    setSelectIdClick(value);

    (async () => {
      var getImage = await getFileImage(
        `BillOfLading/GetImageById?id=${value.id}`
      );

      setImage(getImage);
    })();

    showModalForm();
  };

  return (
    <>
      <section className="content">
        <div className="card">
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                title={"Danh Sách Hình Ảnh Chứng Từ"}
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
            style={{ maxWidth: "100%" }}
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
                {ShowModal === "ShowImage" && (
                  <div>
                    <img
                      src={image}
                      style={{ margin: "auto", left: "0", right: "0" }}
                    />
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
};

export default HandlingImage;
